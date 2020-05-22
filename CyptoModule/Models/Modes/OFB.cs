using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Modes
{
    public class OFB : IMode
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

        public OFB(IBlockCipher cipher, byte[] iv)
        {
            _cipher = cipher;
            IV = iv.ToArray();
            if ( _cipher.IsEncrypting == false )
            {
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
                    Encrypt(input, i, output, i);
            }

        }

        private void Encrypt(byte[] input, int inOff, byte[] output, int outOff)
        {

            _cipher.ProcessBlock(_iv, 0, output, outOff);

            Array.Copy(output, outOff, _iv, 0, _iv.Length);

            for (int i = 0; i < _iv.Length; i++)
            {
                output[outOff + i] ^= input[inOff + i];
            }

        }
    }
}
