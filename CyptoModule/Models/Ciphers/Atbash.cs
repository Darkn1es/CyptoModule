using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Atbash : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyz";

        public override bool HasKey { get => false; }
        public override string CipherName { get => "Шифр Атбаш"; }

        public Atbash()
        {

        }

        public override string Encrypt(string text, string key = null)
        {
            string ciphertext = "";
            foreach (var simb in text)
            {
                char cipherSimb = simb;
                string alphabet = "";
                char tempChar = char.ToLower(simb);
                if (_rusAlphabet.Contains(tempChar))
                {
                    alphabet = _rusAlphabet;
                }
                else if (_engAlphabet.Contains(tempChar))
                {
                    alphabet = _engAlphabet;
                }

                if (alphabet != "")
                {
                    int index = alphabet.IndexOf(tempChar);
                    int newIndex = alphabet.Length - index - 1;
                    cipherSimb = alphabet[newIndex];
                }
                if (char.IsUpper(simb))
                {
                    cipherSimb = char.ToUpper(cipherSimb);
                }
                ciphertext += cipherSimb;
            }
            return ciphertext;
        }

        public override string Decrypt(string ciphertext, string key = null)
        {
            return Encrypt(ciphertext);
        }


    }
}
