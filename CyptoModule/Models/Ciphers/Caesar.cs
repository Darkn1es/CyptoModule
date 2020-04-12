using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Caesar : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyz";

        public Caesar()
        {

        }
        public override bool HasKey { get => true; }

        public override string CipherName { get => "Шифр Цезаря"; }

        public override string Decrypt(string ciphertext, string key = null)
        {
            string result = "";
            int offset = 0;
            if (Int32.TryParse(key, out offset))
            {
                if (offset >= 0)
                {
                    result = ShiftText(ciphertext, -1 * offset);
                }
                else
                {
                    throw new Exception("Ключ должен быть положительным числом");
                }
            }
            else
            {
                throw new Exception("Ключ должен быть положительным числом");
            }
            return result;
        }

        public override string Encrypt(string text, string key = null)
        {
            string result = "";
            int offset = 0;
            if (Int32.TryParse(key, out offset))
            {
                if (offset >= 0)
                {
                    result = ShiftText(text, offset); 
                }
                else
                {
                    throw new Exception("Ключ должен быть положительным числом");
                }
            }
            else
            {
                throw new Exception("Ключ должен быть положительным числом");
            }
            return result;
        }

        private string ShiftText(string text, int offset)
        {
            string result = "";

            foreach (var symb in text)
            {
                char cipherSimb = symb;
                string alphabet = "";
                char tempChar = char.ToLower(symb);

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
                    int newIndex = (((index + offset) % alphabet.Length) + alphabet.Length) % alphabet.Length;
                    cipherSimb = alphabet[newIndex];
                }
                if (char.IsUpper(symb))
                {
                    cipherSimb = char.ToUpper(cipherSimb);
                }
                result += cipherSimb;
            }
            return result;
        }


    }
}
