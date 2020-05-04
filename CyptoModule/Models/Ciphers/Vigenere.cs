using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Vigenere : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyz";
        public override bool HasKey => true;

        public override string CipherName => "Шифр Виженера";

        public override string Decrypt(string ciphertext, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            string alphabet = "";
            key = key.ToLower();
            if (key.All(symb => _rusAlphabet.Contains(symb)))
            {
                alphabet = _rusAlphabet;
            }
            else if (key.All(symb => _engAlphabet.Contains(symb)))
            {
                alphabet = _engAlphabet;
            }
            else
            {
                throw new Exception("Ключ должен состоять из символов одного алфавита");
            }

            string text = "";


            List<int> keyInt = new List<int>();
            foreach (var item in key)
            {
                keyInt.Add(alphabet.IndexOf(item));
            }

            var keyIterator = keyInt.GetEnumerator();

            foreach (var symb in ciphertext)
            {
                char lowerSymb = char.ToLower(symb);
                if (alphabet.Contains(lowerSymb))
                {
                    if (!keyIterator.MoveNext())
                    {
                        keyIterator = keyInt.GetEnumerator();
                        keyIterator.MoveNext();
                    }
                    text += ShiftSymb(symb, -1 * keyIterator.Current);
                }
                else
                {
                    text += symb;
                }
            }
            
            return text;
        }

        public override string Encrypt(string text, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            string alphabet = "";
            key = key.ToLower();
            if (key.All(symb => _rusAlphabet.Contains(symb)))
            {
                alphabet = _rusAlphabet;
            }
            else if (key.All(symb => _engAlphabet.Contains(symb)))
            {
                alphabet = _engAlphabet;
            }
            else
            {
                throw new Exception("Ключ должен состоять из символов одного алфавита");
            }

            string ciphertext = "";


            List<int> keyInt = new List<int>();
            foreach (var item in key)
            {
                keyInt.Add(alphabet.IndexOf(item));
            }

            var keyIterator = keyInt.GetEnumerator();

            foreach (var symb in text)
            {
                char lowerSymb = char.ToLower(symb);

                if (!keyIterator.MoveNext())
                {
                    keyIterator = keyInt.GetEnumerator();
                    keyIterator.MoveNext();
                }
                ciphertext += alphabet.Contains(lowerSymb) ? ShiftSymb(symb, keyIterator.Current) : symb;
            }

            return ciphertext;
        }

        private char ShiftSymb(char symb, int offset)
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
            return cipherSimb;
        }
    }
}
