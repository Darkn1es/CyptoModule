using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class XORcipher : CipherAbstract
    {

        private const string _alphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяabcdefghijklmnopqrstuvwxyz .,?!";

        public override bool HasKey => true;

        public override string CipherName => "Гаммирование";

        public override string Decrypt(string ciphertext, string key = null)
        {

            return Encrypt(ciphertext, key);
        }

        public override string Encrypt(string text, string key = null)
        {
            key = key.ToLower();
            validate(key, text);
            
            string result = "";

            var keyIterator = key.GetEnumerator();

            foreach (var symb in text)
            {
                char lowerSymb = char.ToLower(symb);

                if (_alphabet.Contains(lowerSymb))
                {
                    keyIterator.MoveNext();

                    int index = _alphabet.IndexOf(lowerSymb);
                    int keyByte = _alphabet.IndexOf(keyIterator.Current);

                    char tempChar = _alphabet[keyByte ^ index];
                    result += char.IsUpper(symb) ? char.ToUpper(tempChar) : tempChar;
                }
                else
                {
                    result += symb;
                }

            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <returns></returns>
        public override string GenerateKey(string size)
        {
            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            UInt32 seed = Convert.ToUInt32(currentTime % 64); // get seed;
            int sizeInt = 0;
            string key = "";

            if (int.TryParse(size, out sizeInt))
            {
                if (sizeInt > 0)
                {
                    for (int i = 0; i < sizeInt; i++)
                    {
                        var rand = generator(seed);
                        seed = rand;
                        key += _alphabet[Convert.ToInt32(rand)];
                    }
                }
                else
                {
                    throw new Exception("Введите целое положительное число больше нуля");
                }
            }
            else
            {
                throw new Exception("Введите целое положительное число больше нуля");
            }

            return key; 
        }

        private UInt32 generator(UInt32 x)
        {
            UInt32 result = (17 * x + 13) % 64;
            return result;
        }

        private void validate(string key, string text)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => _alphabet.Contains(symb)))
            {
                throw new Exception("Неверный алфавит ключа");
            }
            if (text.Length > key.Length)
            {
                throw new Exception("Ключ должен быть больше входного текста");
            }
        }
    }
}
