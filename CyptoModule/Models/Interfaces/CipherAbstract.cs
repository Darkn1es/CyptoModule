using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
    public abstract class CipherAbstract
    {
        public abstract bool HasKey { get; }

        public abstract string CipherName { get; }

        public abstract string Encrypt(string text, string key = null);
        public abstract string Decrypt(string ciphertext, string key = null);

        public virtual byte[] Encrypt(byte[] text, byte[] key, string mode)
        {
            throw new NotImplementedException();
        }
        public virtual byte[] Decrypt(byte[] ciphertext, byte[] key, string mode)
        {
            throw new NotImplementedException();
        }

        public virtual string GenerateKey(string size)
        {
            return "Random";
        }
    }
}
