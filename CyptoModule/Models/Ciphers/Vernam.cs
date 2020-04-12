using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Vernam : CipherAbstract
    {

        private string _alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz ,.:!";

        public override bool HasKey => true;

        public override string CipherName => "Шифр Вернама";

        public override string Decrypt(string ciphertext, string key = null)
        {
            return Encrypt(ciphertext, key);
        }

        public override string Encrypt(string text, string key = null)
        {
            validate(key);
            string result = "";
            List<int> keyInt = new List<int>();
            foreach (var item in key)
            {
                keyInt.Add(_alphabet.IndexOf(item));
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
                if (_alphabet.Contains(lowerSymb))
                {
                    int index = _alphabet.IndexOf(lowerSymb);
                    char tempChar = _alphabet[keyIterator.Current ^ index];
                    result += char.IsUpper(symb) ? char.ToUpper(tempChar): tempChar;

                }
                else
                {
                    result += symb;
                }

            }

            return result;
        }

        

        private void validate(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => _alphabet.Contains(symb)))
            {
                throw new Exception("Ключ должен состоять из символов одного алфавита");
            }
        }
    }
}
