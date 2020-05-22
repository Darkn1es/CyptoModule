using CyptoModule.Models.Interfaces;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CyptoModule.ViewModels
{
    public class ModernCipherVM : BindableBase
    {
        private CipherAbstract _cipher;
        public CipherAbstract Cipher 
        {
            get => _cipher;
            set
            {
                _cipher = value;
                if ( _cipher.CipherName == "DES" )
                {
                    Modes.Clear();
                    Modes.Add( "ECB" );
                    Modes.Add( "CBC" );
                    Modes.Add( "CFB" );
                    Modes.Add( "OFB" );
                    SelectedMode = "ECB";
                }
            }
        }
        private string _selectedMode = "ECB";
        public string SelectedMode
        {
            get => _selectedMode;
            set
            {
                _selectedMode = value;
                RaisePropertyChanged( nameof( SelectedMode ) );
            }
        }

        public ObservableCollection<string> Modes { get; set; } = new ObservableCollection<string>();

        private bool _isDoingCipher = false;
        public bool IsDoingCipher
        {
            get => _isDoingCipher;
            set
            {
                _isDoingCipher = value;
                RaisePropertyChanged( nameof( IsDoingCipher ) );
            }
        }

        private string _keyText = "";
        public string KeyText
        {
            get => _keyText;
            set
            {
                _keyText = value;
                RaisePropertyChanged( KeyText );
            }
        }


        private string _inputText = "";
        public string InputText
        {
            get => _inputText;
            set
            {
                _inputText = value;
                RaisePropertyChanged( nameof( InputText ) );
            }
        }

        private bool _canConvertInput = true;
        public bool CanConvertInput
        {
            get => _canConvertInput;
            private set
            {
                _canConvertInput = value;
                RaisePropertyChanged( nameof( CanConvertInput ) );
            }
        }

        private bool _canConvertOutput = true;
        public bool CanConvertOutput
        {
            get => _canConvertOutput;
            private set
            {
                _canConvertOutput = value;
                RaisePropertyChanged( nameof( CanConvertOutput ) );
            }
        }


        private string _hexInputText = "";
        private string _hexOutputText = "";

        private string _outputText = "";
        public string OutputText
        {
            get => _outputText;
            set
            {
                _outputText = value;
                RaisePropertyChanged( nameof( OutputText ) );
            }
        }


        private bool _isHEX = false;
        public bool IsHEX
        {
            get => _isHEX;
            set
            {
                _isHEX = value;
                if ( value == false )
                {
                    try
                    {
                        InputText = HexToString( InputText );
                    }
                    catch ( Exception )
                    {
                        _hexInputText = InputText;
                        CanConvertInput = false;
                        InputText = "Не удается преобразовать в ASCII символы.";
                    }
                    try
                    {
                        OutputText = HexToString( OutputText );
                    }
                    catch ( Exception )
                    {
                        _hexOutputText = OutputText;
                        CanConvertOutput = false;
                        OutputText = "Не удается преобразовать в ASCII символы.";
                    }
                }
                else
                {
                    InputText = CanConvertInput ? StringToHex( InputText ) : _hexInputText;
                    CanConvertInput = true;
                    OutputText = CanConvertOutput ? StringToHex( OutputText ) : _hexOutputText;
                    CanConvertOutput = true;

                }
                RaisePropertyChanged( nameof( IsHEX ) );
            }
        }

        private double _currentProgress = 0.0;
        public double CurrentProgress
        {
            get => _currentProgress;
            set
            {
                _currentProgress = value;
                Percent = Math.Round( value );
                RaisePropertyChanged( nameof( Percent ) );
                RaisePropertyChanged( nameof( CurrentProgress ) );
            }
        }



        public double Percent { get; set; } = 0;

        private string StringToHex(string text)
        {
            byte[] temp = Encoding.Default.GetBytes( text );
            var hexString = BitConverter.ToString( temp );
            hexString = hexString.Replace( "-", "" );
            return hexString;
        }

        private string HexToString(string hex)
        {
            byte[] raw = HexToByteArray( hex );
            return Encoding.Default.GetString(raw);
        }

        private byte[] HexToByteArray(string hex)
        {
            hex = hex.Replace( "-", "" );
            byte[] raw = new byte[ hex.Length / 2 ];
            for ( int i = 0; i < raw.Length; i++ )
            {
                raw[ i ] = Convert.ToByte( hex.Substring( i * 2, 2 ), 16 );
            }
            return raw;
        }

        public ModernCipherVM()
        {
            EncryptCommand = new DelegateCommand( () =>
            {
                try
                {
                    ValidateKey( KeyText );
                    byte[] input;

                    // getting input
                    if ( IsHEX )
                    {
                        ValidateHexString( InputText );
                        input = HexToByteArray( InputText );
                    }
                    else
                    {
                        if ( CanConvertInput )
                        {
                            input = Encoding.Default.GetBytes( InputText );
                        }
                        else
                        {
                            ValidateHexString( _hexInputText );
                            input = HexToByteArray( _hexInputText );
                        }
                    }

                    byte[] key = Encoding.Default.GetBytes( KeyText );

                    byte[] result = Cipher.Encrypt( input, key, SelectedMode );

                    string hexString = BitConverter.ToString( result );
                    hexString = hexString.Replace( "-", "" );

                    if ( IsHEX )
                    {
                        OutputText = hexString;
                    }
                    else
                    {
                        // Костыль нужно переписать
                        IsHEX = true;
                        OutputText = hexString;
                        IsHEX = false;

                    }

                }
                catch ( Exception )
                {
                    MessageBox.Show( "Некоректные данные. Проверьте ключ и текст. Ключ должен состоять из 8 символов" );
                }

            } );


            DecryptCommand = new DelegateCommand( () =>
            {
                try
                {
                    ValidateKey( KeyText );

                    byte[] input;

                    // getting input
                    if ( IsHEX )
                    {
                        ValidateHexString( InputText );
                        input = HexToByteArray( InputText );
                    }
                    else
                    {
                        if ( CanConvertInput )
                        {
                            input = Encoding.Default.GetBytes( InputText );
                        }
                        else
                        {
                            ValidateHexString( _hexInputText );
                            input = HexToByteArray( _hexInputText );
                        }
                    }

                    byte[] key = Encoding.Default.GetBytes( KeyText );

                    byte[] result = Cipher.Decrypt( input, key, SelectedMode );

                    string hexString = BitConverter.ToString( result );
                    hexString = hexString.Replace( "-", "" );

                    if ( IsHEX )
                    {
                        OutputText = hexString;
                    }
                    else
                    {
                        // Костыль нужно переписать
                        IsHEX = true;
                        OutputText = hexString;
                        IsHEX = false;

                    }

                }
                catch ( Exception )
                {
                    MessageBox.Show( "Некоректные данные. Проверьте ключ и текст. Ключ должен состоять из 8 символов" );
                }
            } );
            OpenFileCommand = new DelegateCommand( () =>
            {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "Text|*.txt";

                    if ( openFileDialog.ShowDialog() == true )
                    {
                        string path = openFileDialog.FileName;
                        FileInfo fileInfo = new FileInfo( path );
                        if ( fileInfo.Length > 4096 )
                        {
                            throw new Exception( "Файл слишком большой" );
                        }
                        KeyText = File.ReadAllText( path );
                    }
                }
                catch ( Exception error )
                {
                    MessageBox.Show( error.Message );
                }
            } );

            SaveFileCommand = new DelegateCommand( () =>
            {
                try
                {
                    if ( KeyText == "" )
                    {
                        throw new Exception( "Нет данных для сохранения" );
                    }

                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    if ( saveFileDialog.ShowDialog() == true )
                    {
                        File.WriteAllText( saveFileDialog.FileName, KeyText );
                        MessageBox.Show( "Ключ успешно сохранен в файл" );
                    }
                }
                catch ( Exception error )
                {
                    MessageBox.Show( error.Message );
                }
            } );

            EncryptFileCommand = new DelegateCommand( () =>
            {
                try
                {
                    ValidateKey( KeyText );

                    ICipherFIle cipher = Cipher as ICipherFIle;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    if ( openFileDialog.ShowDialog() == true )
                    {
                        cipher.ProgressChanged += ProgressChanged;
                        byte[] key = Encoding.Default.GetBytes( KeyText );

                        string path = openFileDialog.FileName;
                        _path = path;
                        _bytekey = key;
                        IsDoingCipher = true;

                        Thread thread = new Thread( StartEncrypt );
                        thread.Start();
                    }
                }
                catch ( Exception )
                {
                    MessageBox.Show( "Ошибка в ключе либо в файле. Ключ должен состоять из 8 символов" );
                }
            } );

            DecryptFileCommand = new DelegateCommand( () =>
            {
                try
                {
                    ValidateKey( KeyText );
                    ICipherFIle cipher = Cipher as ICipherFIle;
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    if ( openFileDialog.ShowDialog() == true )
                    {
                        cipher.ProgressChanged += ProgressChanged;

                        byte[] key = Encoding.Default.GetBytes( KeyText );

                        string path = openFileDialog.FileName;
                        _path = path;
                        _bytekey = key;

                        IsDoingCipher = true;
                        Thread thread = new Thread( StartDecrypt );
                        thread.Start();
                    }
                }
                catch ( Exception )
                {
                    MessageBox.Show( "Ошибка в ключе либо в файле. Ключ должен состоять из 8 символов" );
                }
            } );
        }


        public DelegateCommand EncryptCommand { get; }
        public DelegateCommand DecryptCommand { get; }
        public DelegateCommand OpenFileCommand { get; }
        public DelegateCommand SaveFileCommand { get; }
        public DelegateCommand EncryptFileCommand { get; }
        public DelegateCommand DecryptFileCommand { get; }


        private string _path;
        private byte[] _bytekey;
        private void StartEncrypt()
        {
            ICipherFIle cipher = Cipher as ICipherFIle;
            try
            {
                cipher.EncryptFile( _path, _path + ".enc", _bytekey, SelectedMode );
                CurrentProgress = 100;
                MessageBox.Show( "ГОТОВО!" );
            }
            catch ( Exception )
            {
                MessageBox.Show( "Произошла ошибка" );
            }
            cipher.ProgressChanged -= ProgressChanged;
            IsDoingCipher = false;

        }

        private void StartDecrypt()
        {
            ICipherFIle cipher = Cipher as ICipherFIle;
            try
            {
                cipher.DecryptFile( _path, _path.Replace( ".enc", "" ), _bytekey, SelectedMode );
                CurrentProgress = 100;
                MessageBox.Show( "ГОТОВО!" );
            }
            catch ( Exception )
            {
                MessageBox.Show( "Произошла ошибка" );
            }
            
            cipher.ProgressChanged -= ProgressChanged;
            IsDoingCipher = false;
        }

        private void ProgressChanged( double percent )
        {
            CurrentProgress = percent;
        }

        private void ValidateHexString(string hex)
        {
            // TODO

        }

        private void ValidateKey(string key)
        {
            if ( key.Length != 8 )
            {
                throw new Exception( "Ключ должен состоять из 8 символов." );
            }
        }


    }
}
