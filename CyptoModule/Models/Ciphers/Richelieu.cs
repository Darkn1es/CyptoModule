using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Richelieu : CipherAbstract
    {
        public override bool HasKey => true;

        public override string CipherName => "Шифр Ришелье";

        public override string Decrypt(string ciphertext, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            var keyArray = stringToKeyArray(key);
            validate(keyArray, ciphertext);
            string text = "";
            int startBlock = 0;

            foreach (var block in keyArray)
            {
                string textBlock = ciphertext.Substring(startBlock, block.Count);
                startBlock += block.Count;
                StringBuilder tempStr = new StringBuilder(new string(' ', block.Count));

                int i = 0;
                foreach (var item in block)
                {
                    tempStr[item] = textBlock[i++];
                }
                text += tempStr.ToString();
            }
            if (text.Length < ciphertext.Length)
            {
                text += ciphertext.Substring(text.Length);
            }
            return text;
        }

        public override string Encrypt(string text, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            var keyArray = stringToKeyArray(key);
            validate(keyArray, text);
            string cipher = "";
            int startBlock = 0;
            foreach (var block in keyArray)
            {
                string textBlock = text.Substring(startBlock, block.Count);
                startBlock += block.Count;

                foreach (var item in block)
                {
                    cipher += textBlock[item];
                }
            }
            if (cipher.Length < text.Length)
            {
                cipher += text.Substring(cipher.Length);
            }
            return cipher;
        }


        private bool validate(List<List<int>> key, string text)
        {
            int count = 0;
            foreach (var block in key)
            {
                count += block.Count;
                for (int i = 0; i < block.Count; i++)
                {
                    if (!block.Contains(i))
                    {
                        throw new Exception("В блоке должны находиться все числа от 0..n-1, где n длина блока");
                    }
                }
            }
            if (count > text.Length)
            {
                throw new Exception("Ключ больше текста!");
            }
            return true;
        }

        private List<List<int>> stringToKeyArray(string key)
        {
            string pattern = @"\((\d{1,},){0,}\d\)";
            Regex regex = new Regex(pattern);
            List<List<int>> result = new List<List<int>>();

            var matches = regex.Matches(key);
            int len = 0;
            try
            {
                foreach (Match item in matches)
                {
                    var block = new List<int>();
                    len += item.Value.Length;
                    string current = item.Value;
                    Regex regex1 = new Regex(@"[0-9]{1,}");
                    var matches1 = regex1.Matches(current);

                    foreach (Match digit in matches1)
                    {
                        
                        block.Add(int.Parse(digit.Value));
                    }
                    result.Add(block);

                }
            }
            catch (Exception)
            {

                throw new Exception("Введен неправильный ключ");
            }

            if (len != key.Length)
            {
                throw new Exception("Введен неправильный ключ");
            }

            return result;

        }
    }
}
