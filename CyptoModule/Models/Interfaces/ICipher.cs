using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
    public interface ICipher
    {
        bool HasKey { get; }
        //bool IsStringKey { get; }

        string CipherName { get; }

        string Encrypt(string text, string key = null);
        string Decrypt(string ciphertext, string key = null);
    }
}
