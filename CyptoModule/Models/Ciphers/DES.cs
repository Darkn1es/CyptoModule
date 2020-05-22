using CyptoModule.Models.Auxiliary;
using CyptoModule.Models.DES;
using CyptoModule.Models.Interfaces;
using CyptoModule.Models.Modes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class DES : CipherAbstract, ICipherFIle
    {

        public event Action<double> ProgressChanged;


        public override bool HasKey => true;

        public override string CipherName => "DES";

        public override string Decrypt(string ciphertext, string key = null)
        {
            throw new NotImplementedException();
        }

        public override string Encrypt(string text, string key = null)
        {
            throw new NotImplementedException();
        }

        public override byte[] Decrypt(byte[] ciphertext, byte[] key, string mode)
        {
            byte[] result = new byte[ciphertext.Length];
            var des = new DesEngine();
            des.Init(false, new KeyParameter(key.ToArray()));
            //CFB ecb = new CFB(des, key.ToArray());
            IMode chosenMode = ChooseMode( mode, key, des );

            chosenMode.Process(ciphertext, result);
            return RemovePadding(result);
        }

        public override byte[] Encrypt(byte[] text, byte[] key, string mode)
        {
            text = AddPadding(text);
            var des = new DesEngine();
            des.Init(true, new KeyParameter(key.ToArray()));
            IMode chosenMode = ChooseMode( mode, key, des );

            byte[] result = new byte[text.Length];
            chosenMode.Process(text, result);
            return result;
        }

        private byte[] AddPadding(byte[] input)
        {
            int blockSize = 8;

            int paddingSize = 0;
            byte fillingSize = 0;
            if (input.Length % blockSize != 0)
            {
                paddingSize = blockSize - (input.Length % blockSize);
                fillingSize = Convert.ToByte(paddingSize);
            }
            paddingSize += 8;

            List<byte> result = new List<byte>(input);
            result.AddRange(new byte[paddingSize]);
            result[result.Count - 1] = fillingSize;
            return result.ToArray();
        }

        private byte[] RemovePadding(byte[] input)
        {
            List<byte> result = new List<byte>(input);
            int fillingSize = Convert.ToInt32(result[result.Count - 1]);
            int index = result.Count - 8 - fillingSize;
            result.RemoveRange(index, 8 + fillingSize);

            return result.ToArray();
        }
        public void EncryptFile( string inputFile, string outputFile, byte[] key, string mode)
        {
            FileStream instream;
            FileStream outstream;

            try
            {
                double percent = 0.0;
                long current = 0;
                long sizeFile = 0;
                var des = new DesEngine();
                des.Init( true, new KeyParameter( key.ToArray() ) );
                IMode chosenMode = ChooseMode( mode, key, des );
                outstream = File.Open( outputFile, FileMode.Create );
                int chunkSize = 1024*1024;
                using ( instream = File.OpenRead( inputFile ) )
                {
                    sizeFile = instream.Length;
                    ProgressChanged?.Invoke( 0.0 );
                    int byteRead = 0;
                    byte[] input = new byte[ chunkSize ];
                    byte[] output = new byte[ chunkSize ];

                    while ( true )
                    {
                        byteRead = instream.Read( input, 0, chunkSize );
                        if ( byteRead == chunkSize )
                        {
                            chosenMode.Process( input, output );
                            outstream.Write( output, 0, chunkSize );
                        }
                        else
                        {
                            byte[] temp = new byte[ byteRead ];
                            Array.Copy( input, 0, temp, 0, byteRead );
                            input = temp;
                            input = AddPadding( input.ToArray() );
                            output = new byte[ input.Length ];
                            chosenMode.Process( input, output );
                            outstream.Write( output, 0, output.Length );
                            break;
                        }
                        current += byteRead;
                        percent = (double)current / sizeFile;
                        percent *= 100;
                        ProgressChanged?.Invoke( percent );
                    }

                }
            }
            catch ( Exception )
            {
                throw new Exception("Ошибка!");
            }
            outstream?.Close();
            instream?.Close();
        }

        public void DecryptFile( string inputFile, string outputFile, byte[] key, string mode )
        {
            FileStream instream;
            FileStream outstream;
            try
            {
                double percent = 0.0;
                long current = 0;
                long sizeFile = 0;
                var des = new DesEngine();
                des.Init( false, new KeyParameter( key.ToArray() ) );
                IMode chosenMode = ChooseMode( mode, key, des );

                outstream = File.Open( outputFile, FileMode.Create );
                int chunkSize = 1024 * 1024;
                using ( instream = File.OpenRead( inputFile ) )
                {
                    sizeFile = instream.Length;
                    ProgressChanged?.Invoke( 0.0 );
                    int byteRead = 0;
                    byte[] input = new byte[ chunkSize ];
                    byte[] output = new byte[ chunkSize ];
                    byte lastByte = 0;
                    while ( true )
                    {
                        byteRead = instream.Read( input, 0, chunkSize );
                        if ( byteRead == chunkSize )
                        {
                            chosenMode.Process( input, output );
                            lastByte = output[ output.Length - 1 ];
                            outstream.Write( output, 0, chunkSize );
                        }
                        else
                        {
                            if ( byteRead != 0 )
                            {
                                byte[] temp = new byte[ byteRead ];
                                Array.Copy( input, 0, temp, 0, byteRead );
                                input = temp;
                                output = new byte[ input.Length ];
                                chosenMode.Process( input, output );
                                lastByte = output[ output.Length - 1 ];
                                //output = RemovePadding( output.ToArray() );
                                outstream.Write( output, 0, output.Length );
                            }
                            break;


                        }
                        current += byteRead;
                        percent = (double)current / sizeFile;
                        percent *= 100;
                        ProgressChanged?.Invoke( percent );
                    }
                    var size = outstream.Length;
                    long sizeOfFill = Convert.ToInt64( lastByte ) + 8;
                    outstream.SetLength( size - sizeOfFill );
                    outstream.Close();
                    instream.Close();
                }
            }
            catch ( Exception )
            {
                throw new Exception( "Ошибка!" );
            }
            outstream?.Close();
            instream?.Close();
        }

        private IMode ChooseMode(string mode, byte[] key, DesEngine engine)
        {
            IMode chosenMode;
            switch ( mode )
            {
                case "ECB":
                    chosenMode = new ECB( engine );
                    break;
                case "CBC":
                    chosenMode = new CBC( engine, key.ToArray() );
                    break;
                case "CFB":
                    chosenMode = new CFB( engine, key.ToArray() );
                    break;
                case "OFB":
                    chosenMode = new OFB( engine, key.ToArray() );
                    break;
                default:
                    throw new Exception( "Неправильный режим" );
            }

            return chosenMode;
        }
    }
}
