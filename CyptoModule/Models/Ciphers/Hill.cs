using CyptoModule.Models.Auxiliary;
using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CyptoModule.Models.Ciphers
{
    public class Hill : CipherAbstract
    {
        private const string _rusAlphabet = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
        private const string _engAlphabet = "abcdefghijklmnopqrstuvwxyz";
        public override bool HasKey => true;

        public override string CipherName => "Криптосистема Хилла";

        public override string Decrypt(string ciphertext, string key = null)
        {

            string alphabet = validateKey(key);
            key = key.ToLower();

            string result = "";

            int sizeOfMatrix = Convert.ToInt32(Math.Ceiling(Math.Sqrt(key.Length)));

            char fillchar = alphabet[alphabet.Length - 1];

            key = key.PadRight(sizeOfMatrix * sizeOfMatrix, fillchar);

            Matrix keyMatrix = new Matrix(sizeOfMatrix, sizeOfMatrix, alphabet.Length);

            var keyIter = key.GetEnumerator();
            keyMatrix.ProcessFunctionOverData((i, j) =>
            {
                keyIter.MoveNext();
                keyMatrix[i, j] = alphabet.IndexOf(keyIter.Current);
            });
            Matrix inverse;
            try
            {
                inverse = keyMatrix.CreateInvertibleMatrix();
            }
            catch (Exception)
            {

                throw new Exception("Ключ не подходит, так как матрица составленная по ключу не обратима");
            }

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

            if ((tempText.Length % keyMatrix.N) != 0)
            {
                int fillSize = keyMatrix.N - (tempText.Length % keyMatrix.N);
                tempText = tempText.PadRight(tempText.Length + fillSize, fillchar);

                for (int i = 0; i < fillSize; i++)
                {
                    ptrs.Add(ciphertext.Length + i);
                }

                ciphertext = ciphertext.PadRight(ciphertext.Length + fillSize, fillchar);
            }

            int interval = keyMatrix.N;

            for (int i = 0; i < tempText.Length; i += interval)
            {
                string ngram = tempText.Substring(i, interval);
                Matrix ngramMatrix = new Matrix(interval, 1, alphabet.Length);
                for (int j = 0; j < interval; j++)
                {
                    ngramMatrix[j, 0] = alphabet.IndexOf(char.ToLower(ngram[j]));
                }

                var resultMatrix = inverse * ngramMatrix;

                for (int j = 0; j < interval; j++)
                {
                    char newchar = alphabet[resultMatrix[j, 0]];

                    result += char.IsUpper(ngram[j]) ? char.ToUpper(newchar) : newchar;
                }
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
            string alphabet = validateKey(key);
            key = key.ToLower();

            string result = "";

            int sizeOfMatrix = Convert.ToInt32(Math.Ceiling(Math.Sqrt(key.Length)));

            char fillchar = alphabet[alphabet.Length - 1];

            key = key.PadRight(sizeOfMatrix * sizeOfMatrix, fillchar);

            Matrix keyMatrix = new Matrix(sizeOfMatrix, sizeOfMatrix, alphabet.Length);

            var keyIter = key.GetEnumerator();
            keyMatrix.ProcessFunctionOverData((i, j) =>
            {
                keyIter.MoveNext();
                keyMatrix[i, j] = alphabet.IndexOf(keyIter.Current);
            });
            try
            {
                var inverse = keyMatrix.CreateInvertibleMatrix();
            }
            catch (Exception)
            {

                throw new Exception("Ключ не подходит, так как матрица составленная по ключу не обратима");
            }

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

            if ((tempText.Length % keyMatrix.N) != 0)
            {
                int fillSize = keyMatrix.N - (tempText.Length % keyMatrix.N);
                tempText = tempText.PadRight(tempText.Length + fillSize, fillchar);

                for (int i = 0; i < fillSize; i++)
                {
                    ptrs.Add(text.Length + i);
                }

                text = text.PadRight(text.Length + fillSize, fillchar);
            }

            int interval = keyMatrix.N;

            for (int i = 0; i < tempText.Length; i += interval)
            {
                string ngram = tempText.Substring(i, interval);
                Matrix ngramMatrix = new Matrix(interval, 1, alphabet.Length);
                for (int j = 0; j < interval; j++)
                {
                    ngramMatrix[j, 0] = alphabet.IndexOf(char.ToLower(ngram[j]));
                }

                var resultMatrix = keyMatrix * ngramMatrix;

                for (int j = 0; j < interval; j++)
                {
                    char newchar = alphabet[resultMatrix[j, 0]];

                    result += char.IsUpper(ngram[j]) ? char.ToUpper(newchar) : newchar;
                }
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

        private string validateKey(string key)
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
            return alphabet;
        }

    }
}
