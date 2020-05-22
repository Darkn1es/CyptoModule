using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Modes
{
    public class ECB : IMode
    {
        private IBlockCipher _cipher;
        public ECB(IBlockCipher cipher)
        {
            _cipher = cipher;
        }

        public byte[] IV { set => throw new NotImplementedException(); }

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

            for (int i = 0; i < input.Length; i+= blockSize)
            {
                _cipher.ProcessBlock(input, i, output, i);
            }
        }


    }
}
