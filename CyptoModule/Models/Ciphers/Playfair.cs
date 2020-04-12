using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Playfair : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдежзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghiklmnopqrstuvwxyz";

        public override bool HasKey => true;

        public override string CipherName => "Шифр Плейфера";

        public override string Decrypt(string ciphertext, string key = null)
        {
            string result = "";
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            string alphabet = "";
            key = key.ToLower();

            key = key.Replace("j", "i");
            key = key.Replace("ё", "е");

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

            string tempKey = String.Join("", key.Distinct());
            foreach (var item in tempKey)
            {

                alphabet = alphabet.Replace(item.ToString(), "");
            }

            alphabet = tempKey + alphabet;

            string tempText = ""; // tempText will contain only alphabet letters

            List<int> ptrs = new List<int>();

            // choose correct letters from open text
            for (int i = 0; i < ciphertext.Length; i++)
            {
                if (alphabet.Contains(char.ToLower(ciphertext[i])))
                {
                    ptrs.Add(i);
                    tempText += ciphertext[i];
                }
            }

            // for every bigram do rule
            for (int i = 0; i < tempText.Length; i += 2)
            {
                string bigram = "" + tempText[i];
                if (i + 1 < tempText.Length)
                {
                    bigram += tempText[i + 1];
                }
                result += doRule(bigram, alphabet, false);
            }

            StringBuilder stringBuilder = new StringBuilder(ciphertext);
            CharEnumerator iter = result.GetEnumerator();
            foreach (var ptr in ptrs)
            {
                iter.MoveNext();
                stringBuilder[ptr] = iter.Current;
            }

            return stringBuilder.ToString();
        }

        public override string Encrypt(string text, string key = null)
        {
            string result = "";
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            string alphabet = "";
            key = key.ToLower();

            key = key.Replace("j", "i");
            key = key.Replace("ё", "е");

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

            string tempKey = String.Join("", key.Distinct());
            foreach (var item in tempKey)
            {

                alphabet = alphabet.Replace(item.ToString(), "");
            }

            alphabet = tempKey + alphabet;


            string tempText = ""; // tempText will contain only alphabet letters

            List<int> ptrs = new List<int>();

            // choose correct letters from open text
            for (int i = 0; i < text.Length; i++)
            {
                if (alphabet.Contains(char.ToLower(text[i])))
                {
                    ptrs.Add(i);
                    tempText += text[i];
                }
            }

            // for every bigram do rule
            for (int i = 0; i < tempText.Length; i += 2)
            {
                string bigram = "" + tempText[i];
                if (i + 1 < tempText.Length)
                {
                    bigram += tempText[i + 1];
                }
                result += doRule(bigram, alphabet);
            }

            StringBuilder stringBuilder = new StringBuilder(text);
            CharEnumerator iter = result.GetEnumerator();
            foreach (var ptr in ptrs)
            {
                iter.MoveNext();
                stringBuilder[ptr] = iter.Current;
            }

            return stringBuilder.ToString(); 
        }

        private string doRule(string bigram, string alphabet, bool isEncrypt = true)
        {
            string result = "";
            string temp = bigram.ToLower();
            int size = Convert.ToInt32(Math.Sqrt(alphabet.Length));  // count of rows and cols in matrix

            int index = alphabet.IndexOf(temp[0]);

            int x1 = index / size;
            int y1 = index % size;

            int x2 = 0;
            int y2 = 0;

            int shift = isEncrypt ? 1 : size - 1; // choose +1 or -1 mod size;

            if (temp.Length == 2)
            {
                index = alphabet.IndexOf(temp[1]);

                x2 = index / size;
                y2 = index % size;
            }

            if ((temp.Length == 1) || (temp[0] == temp[1]))
            {
                y1 = (y1 + shift) % size;
                result += alphabet[x1 * size + y1];
                if (temp.Length == 2)
                {
                    result += result[0];
                }
            }
            else if (x1 == x2)
            {
                y1 = (y1 + shift) % size;
                result += alphabet[x1 * size + y1];

                y2 = (y2 + shift) % size;
                result += alphabet[x2 * size + y2];
            }
            else if (y1 == y2)
            {
                x1 = (x1 + shift) % size;
                result += alphabet[x1 * size + y1];

                x2 = (x2 + shift) % size;
                result += alphabet[x2 * size + y2];
            }
            else
            {
                int tempY = y1;
                y1 = y2;
                y2 = tempY;

                result += alphabet[x1 * size + y1];
                result += alphabet[x2 * size + y2];
            }

            // Restore case
            StringBuilder stringBuilder = new StringBuilder(result);
            for (int i = 0; i < bigram.Length; i++)
            {
                if (char.IsUpper(bigram[i]))
                {
                    stringBuilder[i] = char.ToUpper(stringBuilder[i]);
                }
            }

            return stringBuilder.ToString();
        }


    }
}
