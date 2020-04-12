using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Gronsfeld : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyz";

        public override bool HasKey => true;

        public override string CipherName => "Шифр Гронсфельда";

        public override string Decrypt(string ciphertext, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => char.IsDigit(symb)))
            {
                throw new Exception("Ключ может состоять только из цифр");
            }

            string text = "";

            var keyIterator = key.GetEnumerator();

            foreach (var symb in ciphertext)
            {
                
                if (!keyIterator.MoveNext())
                {
                    keyIterator = key.GetEnumerator();
                    keyIterator.MoveNext();
                }
                text += ShiftSymb(symb, -1 * int.Parse(keyIterator.Current.ToString()));
            }
            return text;

        }

        public override string Encrypt(string text, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => char.IsDigit(symb)))
            {
                throw new Exception("Ключ может состоять только из цифр");
            }

            string ciphertext = "";

            var keyIterator = key.GetEnumerator();
            foreach (var symb in text)
            {

                if (!keyIterator.MoveNext())
                {
                    keyIterator = key.GetEnumerator();
                    keyIterator.MoveNext();
                }
                ciphertext += ShiftSymb(symb, int.Parse(keyIterator.Current.ToString()));
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
