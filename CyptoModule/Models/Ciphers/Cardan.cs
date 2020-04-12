using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Cardan : CipherAbstract
    {
        public override bool HasKey => true;

        public override string CipherName => "Шифр Кардано";




        public override string Decrypt(string ciphertext, string key = null)
        {
            string text = "";

            var keyVector = stringToKeyArray(key);
            int keyValue = (int)Math.Sqrt((keyVector.Count * 4));
            if ((keyValue * keyValue) < text.Length)
            {
                throw new Exception("Сгенирируйте решетку побольше!");
            }
            keyVector = keyVector.OrderBy(vector => vector.X).ThenBy(vector => vector.Y).ToList();

            int[,] matrixDecrypt = new int[keyValue, keyValue];
            string[,] matrix = new string[keyValue, keyValue];

            int index = 0;
            for (int i = 0; i < 4; i++)
            {
                foreach (var item in keyVector)
                {
                    int x = (int)item.X;
                    int y = (int)item.Y;
                    matrixDecrypt[x, y] = index++;
                }
                keyVector = RotateArray(keyVector);
            }

            var iter = ciphertext.GetEnumerator();
            for (int i = 0; i < keyValue; i++)
            {
                for (int j = 0; j < keyValue; j++)
                {
                    if (matrixDecrypt[i, j] < ciphertext.Length)
                    {
                        iter.MoveNext();
                        matrix[i, j] = iter.Current.ToString();
                    }
                }
            }

            for (int i = 0; i < 4; i++)
            {
                foreach (var item in keyVector)
                {
                    int x = (int)item.X;
                    int y = (int)item.Y;
                    text += matrix[x, y];
                }
                keyVector = RotateArray(keyVector);
            }

            return text;
        }

        public override string Encrypt(string text, string key = null)
        {
            string cipher = "";
            var keyVector = stringToKeyArray(key);

            int keyValue = (int)Math.Sqrt((keyVector.Count * 4));
            if ((keyValue * keyValue) < text.Length)
            {
                throw new Exception("Сгенирируйте решетку побольше!");
            }
            string[,] matrix = new string[keyValue, keyValue];

            for (int i = 0; i < text.Length; i++)
            {
                int index = i % keyVector.Count;
                int x = (int)keyVector[index].X;
                int y = (int)keyVector[index].Y;

                matrix[x, y] = text[i].ToString();


                if (i % keyVector.Count == (keyVector.Count - 1))
                {
                    keyVector = RotateArray(keyVector);
                }
            }
            foreach (var item in matrix)
            {
                cipher += item;
            }

            return cipher;
        }

        public override string GenerateKey(string size)
        {
            int m = 0;
            if (Int32.TryParse(size, out m))
            {
                if ((m > 2) && (m % 2 == 0))
                {
                    var key = generateKey(m);
                    return arrayKeyToString(key);
                }
                else
                {
                    throw new Exception("Введите число четное целое число больше 2");
                }
            }
            else
            {
                throw new Exception("Введите число четное целое число больше 2");
            }

        }

        private List<Vector2> generateKey(int size)
        {
            Random rand = new Random();
            int maxValue = (size / 2) * (size / 2);
            List<Vector2> graps = new List<Vector2>(size / 2);
            for (int i = 0; i < maxValue; i++)
            {
                int quad = rand.Next(4);
                int x = i / (size / 2);
                int y = i % (size / 2);
                while (quad > 0)
                {
                    Rotate(ref x, ref y, size - 1);
                    quad--;
                }
                graps.Add(new Vector2(x, y));
            }

            graps = graps.OrderBy(vector => vector.X).ThenBy(vector => vector.Y).ToList(); // сортировка
            return graps;
        }

        private void Rotate(ref int x, ref int y, int maxValue)
        {
            int newX = y;
            int newY = maxValue - x;
            x = newX;
            y = newY;
        }

        private List<Vector2> RotateArray(List<Vector2> input)
        {
            List<Vector2> result = new List<Vector2>();
            int keyValue = (int)Math.Sqrt((input.Count * 4));
            for (int j = 0; j < input.Count; j++)
            {
                var item = input[j];
                int newX = (int)item.X;
                int newY = (int)item.Y;
                Rotate(ref newX, ref newY, keyValue - 1);
                item.X = newX;
                item.Y = newY;
                result.Add(item);
            }
            result = result.OrderBy(vector => vector.X).ThenBy(vector => vector.Y).ToList(); // сортировка
            return result;
        }

        private string arrayKeyToString(List<Vector2> key)
        {
            string result = "";
            foreach (var item in key)
            {
                result += "(" + item.X.ToString() + "," + item.Y.ToString() + ")";
            }

            return result;
        }

        private List<Vector2> stringToKeyArray(string key)
        {
            string pattern = @"\([0-9]{1,},[0-9]{1,}\)";
            Regex regex = new Regex(pattern);
            List<Vector2> result = new List<Vector2>();
            var matches = regex.Matches(key);
            int len = 0;
            foreach (Match item in matches)
            {
                len += item.Value.Length;
                string current = item.Value;
                Regex regex1 = new Regex(@"[0-9]{1,}");
                var matches1 = regex1.Matches(current);
                int x = int.Parse(matches1[0].Value);
                int y = int.Parse(matches1[1].Value);

                result.Add(new Vector2(x, y));
            }
            if (len != key.Length)
            {
                throw new Exception("Введен неправильный ключ");
            }

            return result;

        }


    }
}
