using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Alberti : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюяАБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        private const string _encRusAlphabet = "вФВрТЬЗШЧзРоОЯИЩАэяСЫПЁжтгсЮнГЛЭкъилщюёЦдчЙыаЖцбКьфйхуЪДепЕшХНМУБм";

        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string _encEngAlphabet = "TPtwLQKXzUvjMnENFuRyOGZgfhBYpVxJHCdlDsaeoqAImrWkSibc";
        public override bool HasKey => true;

        public override string CipherName => "Диск Альберти";

        public override string Decrypt(string ciphertext, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => char.IsDigit(symb)))
            {
                throw new Exception("Ключ может состоять только из цифры больше 0");
            }

            int shift = int.Parse(key);
            if (shift <= 0)
            {
                throw new Exception("Ключ может состоять только из цифры больше 0");
            }

            string result = "";
            string encEngAlphabet = _encEngAlphabet;
            string encRusAlphabet = _encRusAlphabet;

            foreach (var letter in ciphertext)
            {
                string alphabet = "";
                string encAlphabet = "";
                char openText = letter;
                if (_engAlphabet.Contains(letter))
                {
                    alphabet = _engAlphabet;
                    encAlphabet = RotateRightString(encEngAlphabet, shift);
                    encEngAlphabet = encAlphabet;

                }
                else if (_rusAlphabet.Contains(letter))
                {
                    alphabet = _rusAlphabet;
                    encAlphabet = RotateRightString(encRusAlphabet, shift);
                    encRusAlphabet = encAlphabet;
                }

                if (alphabet != "")
                {
                    int index = encAlphabet.IndexOf(letter);
                    openText = alphabet[index];
                }

                result += openText;
            }

            return result;

            throw new NotImplementedException();
        }

        public override string Encrypt(string text, string key = null)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new Exception("Введите ключ");
            }
            if (!key.All(symb => char.IsDigit(symb)))
            {
                throw new Exception("Ключ может состоять только из цифры больше 0");
            }

            int shift = int.Parse(key);
            if (shift <= 0)
            {
                throw new Exception("Ключ может состоять только из цифры больше 0");
            }

            string result = "";
            string encEngAlphabet = _encEngAlphabet;
            string encRusAlphabet = _encRusAlphabet;

            foreach (var letter in text)
            {
                string alphabet = "";
                string encAlphabet = "";
                char cipher = letter;
                if (_engAlphabet.Contains(letter))
                {
                    alphabet = _engAlphabet;
                    encAlphabet = RotateRightString(encEngAlphabet, shift);
                    encEngAlphabet = encAlphabet;

                } else if (_rusAlphabet.Contains(letter)) 
                {
                    alphabet = _rusAlphabet;
                    encAlphabet = RotateRightString(encRusAlphabet, shift);
                    encRusAlphabet = encAlphabet;
                }

                if (alphabet != "")
                {
                    int index = alphabet.IndexOf(letter);
                    cipher = encAlphabet[index];
                }

                result += cipher;
            }

            return result;
        }

        private string RotateRightString(string text, int shift)
        {
            shift = shift % text.Length;
            return text.Substring(shift, text.Length - shift) + text.Substring(0, shift);
        }

    }
}
