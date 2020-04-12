using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Polybius : CipherAbstract
    {
        public override bool HasKey => true;

        public override string CipherName => "Квадрат Полибия";

        public override string Decrypt(string ciphertext, string key = null)
        {
            string text = "";
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Ключ не должен быть пустым");
            }
            key = key.ToLower();
            string alphabet = chooseAlphabet(key);
            if (alphabet == null)
            {
                throw new Exception("Ключ должен содержать символы одного языка");
            }
            string[,] square = getSquare(key, alphabet);

            foreach (var symb in ciphertext)
            {
                int i = 0;
                int j = 0;
                char dechar = symb;
                if (getPosition(square, symb.ToString(), out j, out i))
                {
                    do
                    {
                        i = (i - 1 + square.GetLength(0)) % square.GetLength(0);
                    } while (string.IsNullOrEmpty(square[i, j]));
                    dechar = square[i, j][0];
                    if (char.IsUpper(symb))
                    {
                        dechar = char.ToUpper(dechar);
                    }
                }
                text += dechar;
            }

            return text;
        }

        public override string Encrypt(string text, string key = null)
        {
            string ciphertext = "";
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Ключ не должен быть пустым");
            }
            key = key.ToLower();
            string alphabet = chooseAlphabet(key);
            if (alphabet == null)
            {
                throw new Exception("Ключ должен содержать символы одного языка");
            }

            string[,] square = getSquare(key, alphabet);

            foreach (var symb in text)
            {
                int i = 0;
                int j = 0;
                char cipher = symb;
                if (getPosition(square, symb.ToString(), out j, out i))
                {
                    do
                    {
                        i = (i + 1) % square.GetLength(0);
                    } while (string.IsNullOrEmpty(square[i, j]));
                    cipher = square[i, j][0];
                    if (char.IsUpper(symb))
                    {
                        cipher = char.ToUpper(cipher);
                    }
                }
                ciphertext += cipher;
            }

            return ciphertext;
        }

        private bool getPosition(string[,] square, string symb, out int x, out int y)
        {
            for (int i = 0; i < square.GetLength(0); i++)
            {
                for (int j = 0; j < square.GetLength(0); j++)
                {
                    if (symb.ToLower() == square[i, j])
                    {
                        y = i;
                        x = j;
                        return true;
                    }
                }
            }
            x = 0;
            y = 0;
            return false;
        }

        private string[,] getSquare(string key, string alphabet)
        {
            string temp = String.Join("", key.Distinct());
            foreach (var item in temp)
            {
                
                alphabet = alphabet.Replace(item.ToString(), "");
            }

            alphabet = temp + alphabet;
            int size = Convert.ToInt32(Math.Ceiling(Math.Sqrt(alphabet.Length)));
            string[,] square = new string[size, size];

            int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (k == alphabet.Length)
                    {
                        break;
                    }
                    square[i, j] = alphabet[k++].ToString();
                }
            }
            return square;
        }

        private string chooseAlphabet(string key)
        {
            string rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
            string engAlphabet = "abcdefghijklmnopqrstuvwxyz";

            if (key.All(c => rusAlphabet.Contains(c)))
            {
                return rusAlphabet;
            }
            else if (key.All(c => engAlphabet.Contains(c)))
            {
                return engAlphabet;
            }
            return null;
        }
    }
}
