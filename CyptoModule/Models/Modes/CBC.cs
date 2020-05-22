using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Modes
{
    public class CBC : IMode
    {

        private IBlockCipher _cipher;
        private byte[] _iv;
        public byte[] IV
        {
            set
            {
                if (value.Length != _cipher.GetBlockSize())
                {
                    throw new Exception("Неправильный вектор инициализации");
                }
                _iv = value;
            }
        }

        public CBC(IBlockCipher cipher, byte[] iv)
        {
            _cipher = cipher;
            IV = iv.ToArray();

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
                if (_cipher.IsEncrypting)
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
            var temp = new byte[_cipher.GetBlockSize()];

            Array.Copy(input, inOff, temp, 0, _cipher.GetBlockSize());

            _cipher.ProcessBlock(input, inOff, output, outOff);

            for (int i = 0; i < _iv.Length; i++)
            {
                output[outOff + i] ^= _iv[i];
            }
            _iv = temp;

        }

        private void Encrypt(byte[] input, int inOff, byte[] output, int outOff)
        {
            for (int i = 0; i < _iv.Length; i++)
            {
                input[inOff + i] ^= _iv[i];
            }

            _cipher.ProcessBlock(input, inOff, output, outOff);

            Array.Copy(output, outOff, _iv, 0, _iv.Length);

        }

    }
}
