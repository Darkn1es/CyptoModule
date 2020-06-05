using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Modes
{
    public class CFB : IMode
    {
        private IBlockCipher _cipher;

        private byte[] _iv;
        private bool _isEncrypt = true;

        public byte[] IV
        {
            set
            {
                if (value.Length < _cipher.GetBlockSize())
                {
                    throw new Exception("Неправильный вектор инициализации");
                }
                else if ( value.Length > _cipher.GetBlockSize() )
                {
                    byte[] temp = new byte[ _cipher.GetBlockSize() ];
                    Array.Copy( value, 0, temp, 0, _cipher.GetBlockSize() );
                    _iv = temp;
                }
                else
                {
                    _iv = value;
                }
            }
        }

        public CFB(IBlockCipher cipher, byte[] iv)
        {
            _cipher = cipher;
            IV = iv.ToArray();
            if ( _cipher.IsEncrypting == false )
            {
                _isEncrypt = false;
                _cipher.Init( true, _cipher.ByteKey );
            }

        }

        public void Process(byte[] input, byte[] output)
        {
            if (input.Length % _cipher.GetBlockSize() != 0)
            {
                throw new Exception("Входной поток не кратен размеру блока");
            }
            if (input.Length > output.Length)
            {
                throw new Exception("Входной поток больше выходного");
            }

            int blockSize = _cipher.GetBlockSize();

            for (int i = 0; i < input.Length; i += blockSize)
            {
                if ( _isEncrypt )
                {
                    Encrypt(input, i, output, i);
                }
                else
                {
                    Decrypt(input, i, output, i);
                }
            }

        }

        private void Decrypt(byte[] input, int inOff, byte[] output, int outOff)
        {
            _cipher.ProcessBlock(_iv, 0, output, outOff);

            for (int i = 0; i < _iv.Length; i++)
            {
                output[outOff + i] ^= input[inOff + i];
            }

            Array.Copy(input, inOff, _iv, 0, _iv.Length);
        }

        private void Encrypt(byte[] input, int inOff, byte[] output, int outOff)
        {

            _cipher.ProcessBlock(_iv, 0, output, outOff);

            for (int i = 0; i < _iv.Length; i++)
            {
                output[outOff + i] ^= input[inOff + i];
            }

            Array.Copy(output, outOff, _iv, 0, _iv.Length);
        }
    }
}
