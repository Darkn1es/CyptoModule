using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Hill : CipherAbstract
    {
        public override bool HasKey => true;

        public override string CipherName => "Криптосистема Хилла";

        public override string Decrypt(string ciphertext, string key = null)
        {
            throw new NotImplementedException();
        }

        public override string Encrypt(string text, string key = null)
        {
            throw new NotImplementedException();
        }
    }
}
