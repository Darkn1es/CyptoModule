using CyptoModule.Models.Ciphers;
using CyptoModule.Models.Interfaces;
using CyptoModule.Views;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CyptoModule.ViewModels
{
    public class MainVM : BindableBase
    {

        #region Pages

        private Page _currentPageContent;
        public Page CurrentPageContent
        {
            get
            {
                return _currentPageContent;
            }
            private set
            {
                _currentPageContent = value;
                RaisePropertyChanged(nameof(CurrentPageContent));
            }
        }
        private Page _currentMenuPageContent;
        public Page CurrentMenuPageContent
        {
            get
            {
                return _currentMenuPageContent;
            }
            private set
            {
                _currentMenuPageContent = value;
                RaisePropertyChanged(nameof(CurrentMenuPageContent));
            }
        }


        private Page _keyTextPage;
        private Page _cardanoPage;

        private Page _menuChiphersPage;

        #endregion

        private string _chosenChipher = "";
        public string ChosenChipher
        {
            get => _chosenChipher;
            set
            {
                _chosenChipher = value;
                RaisePropertyChanged(nameof(ChosenChipher));
            }
        }

        public bool IsVisibleKeyBox 
        { 
            get => _isVisibleKeyBox;
            set
            {
                _isVisibleKeyBox = value;
                RaisePropertyChanged(nameof(IsVisibleKeyBox));
            }
        }
        private bool _isVisibleKeyBox = false;


        private CipherAbstract _currentCipher;
        public CipherAbstract CurrentCipher
        {
            get => _currentCipher;
            set
            {
                _currentCipher = value;
                UpdateWindow();
                //RaisePropertyChanged(nameof(CurrentCipher));
            }
        }

        private ObservableCollection<CipherAbstract> _ciphers = new ObservableCollection<CipherAbstract>();
        public ReadOnlyObservableCollection<CipherAbstract> Ciphers;

        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                RaisePropertyChanged(nameof(InputText));
            }
        }

        private string _outputText = "";
        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                RaisePropertyChanged(nameof(OutputText));
            }
        }

        private string _keyText = "";
        public string KeyText
        {
            get => _keyText;
            set
            {
                _keyText = value;
                RaisePropertyChanged(nameof(KeyText));
            }
        }

        private string _keySizeText = "";
        public string KeySizeText
        {
            get => _keySizeText;
            set
            {
                _keySizeText = value;
                RaisePropertyChanged(nameof(KeySizeText));
            }
        }


        #region Commands

        public DelegateCommand EncryptCommand { get; }
        public DelegateCommand DecryptCommand { get; }
        public DelegateCommand GenerateCommand { get; }
        public DelegateCommand<string> ChangeCipherCommand { get; }
        public DelegateCommand ReadFromFileCommand { get; }
        public DelegateCommand SaveToFileCommand { get; }

        #endregion

        public MainVM()
        {
            #region Init ciphers
            _ciphers.Add(new Atbash());
            _ciphers.Add(new Caesar());
            _ciphers.Add(new Scytale());
            _ciphers.Add(new Polybius());
            _ciphers.Add(new Cardan());
            _ciphers.Add(new Richelieu());
            _ciphers.Add(new Alberti());
            _ciphers.Add(new Gronsfeld());
            _ciphers.Add(new Vigenere());
            #endregion

            Ciphers = new ReadOnlyObservableCollection<CipherAbstract>(_ciphers);

            CurrentCipher = Ciphers[0];


            _keyTextPage = new Views.Pages.KeyTextPage();
            _cardanoPage = new Views.Pages.CardanPage();
            _menuChiphersPage = new Views.Pages.MenuCiphersPage();

            _keyTextPage.DataContext = this;
            _cardanoPage.DataContext = this;
            _menuChiphersPage.DataContext = this;

            CurrentPageContent = _keyTextPage;
            CurrentMenuPageContent = _menuChiphersPage;


            EncryptCommand = new DelegateCommand(() =>
            {
                try
                {
                    if (InputText == "")
                    {
                        throw new Exception("Входной текст не должен быть пустым.");
                    }
                    OutputText = CurrentCipher.Encrypt(InputText, KeyText);
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Неверный формат ключа");
                }
                catch (Exception error)
                {
                    // Прости MVVM
                    MessageBox.Show(error.Message);
                }
            });

            DecryptCommand = new DelegateCommand(() => // (1,0)(2,1)(3,0)(3,2) // еитрП !мрив
            {
                try
                {
                    if (InputText == "")
                    {
                        throw new Exception("Входной текст не должен быть пустым.");
                    }
                    OutputText = CurrentCipher.Decrypt(InputText, KeyText);
                }
                catch (IndexOutOfRangeException)
                {
                    MessageBox.Show("Неверный формат ключа");
                }
                catch (Exception error)
                {
                    // Прости MVVM x2
                    MessageBox.Show(error.Message);
                }
            });

            GenerateCommand = new DelegateCommand(() =>
            {
                try
                {
                    if (KeySizeText == "")
                    {
                        throw new Exception("Размер ключа не должен быть пустым.");
                    }
                    KeyText = CurrentCipher.GenerateKey(KeySizeText);
                }
                catch (Exception error)
                {
                    // Прости MVVM x3
                    MessageBox.Show(error.Message);
                }
            });

            ChangeCipherCommand = new DelegateCommand<string>((string cipherName) =>
            {
                try
                {
                    foreach (var cipher in Ciphers)
                    {
                        if (cipher.CipherName == cipherName)
                        {
                            CurrentCipher = cipher;
                            break;
                        }
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            });

            SaveToFileCommand = new DelegateCommand(() =>
            {
                try
                {
                    if (OutputText == "")
                    {
                        throw new Exception("Нет данных для сохранения");
                    }
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if (saveFileDialog.ShowDialog() == true)
                    {
                        File.WriteAllText(saveFileDialog.FileName, OutputText);
                        MessageBox.Show("Результат успешно сохранен в файл");
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            });

            ReadFromFileCommand = new DelegateCommand(() =>
            {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    if (openFileDialog.ShowDialog() == true)
                    {
                        InputText = File.ReadAllText(openFileDialog.FileName);
                    }
                }
                catch (Exception error)
                {
                    MessageBox.Show(error.Message);
                }
            });



        }

        private void UpdateWindow()
        {
            IsVisibleKeyBox = CurrentCipher.HasKey;
            KeyText = "";
            InputText = "";
            OutputText = "";
            ChosenChipher = CurrentCipher.CipherName;

            if (CurrentCipher.CipherName == "Шифр Кардано")
            {
                CurrentPageContent = _cardanoPage;
            }
            else
            {
                CurrentPageContent = _keyTextPage;
            }

        }
    }
}
