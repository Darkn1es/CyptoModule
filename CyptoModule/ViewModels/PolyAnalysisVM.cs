using CyptoModule.Models.Ciphers;
using CyptoModule.Models.Interfaces;
using CyptoModule.Models.Poly;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CyptoModule.ViewModels
{
    public class PolyAnalysisVM : BindableBase
    {
        private string _text = "";
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        private string _keySize = "";
        public string KeySize
        {
            get => _keySize;
            set
            {
                _keySize = value;
                RaisePropertyChanged(nameof(KeySize));
            }
        }

        private string _key = "";
        public string Key
        {
            get => _key;
            set
            {
                _key = value;
                RaisePropertyChanged(nameof(Key));
            }
        }

        private string _language = "Русский";
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                RaisePropertyChanged(nameof(Language));
            }
        }

        private string _decryptedText = "";
        public string DecryptedText
        {
            get => _decryptedText;
            set
            {
                _decryptedText = value;
                RaisePropertyChanged(nameof(DecryptedText));
            }
        }

        public DelegateCommand AnalysisCommand { get; }
        public DelegateCommand DoVigenereCommand { get; }


        public PolyAnalysisAbstract CurrentPolyAnalysis { get; set; }

        public ObservableCollection<PolyAnalysisAbstract> Methods { get; set; }

        public PolyAnalysisVM()
        {
            Methods = new ObservableCollection<PolyAnalysisAbstract>();
            Methods.Add(new IOC());
            Methods.Add(new Autocorrelation());
            Methods.Add(new Kasiski());

            CurrentPolyAnalysis = Methods[0];
            RaisePropertyChanged(nameof(CurrentPolyAnalysis));

            AnalysisCommand = new DelegateCommand(() =>
            {
                try
                {
                    int keySize = 0;
                    PolyAnalysisAbstract.Language language = Language == "Русский" ? PolyAnalysisAbstract.Language.RUSSIAN : PolyAnalysisAbstract.Language.ENGLISH;
                    keySize = CurrentPolyAnalysis.DoAnalysis(Text, language);
                    KeySize = keySize.ToString();
                    Key = CurrentPolyAnalysis.PredictKey(Text, Convert.ToInt32(KeySize), language);
                }
                catch (Exception)
                {
                    MessageBox.Show("Некоррекстный ввод");
                }


            });

            DoVigenereCommand = new DelegateCommand(() =>
            {
                try
                {
                    Vigenere vigenere = new Vigenere();
                    DecryptedText = vigenere.Decrypt(Text, Key);
                }
                catch (Exception)
                {
                    MessageBox.Show("Некоррекстный ввод");
                }
            });
        }
    }
}
