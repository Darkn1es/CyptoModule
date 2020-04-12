using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Ciphers
{
    public class Scytale : CipherAbstract
    {
        public override bool HasKey { get => true; }

        public override string CipherName { get => "Шифр Скитала"; }

        public override string Decrypt(string ciphertext, string key = null)
        {
            string text = "";
            int m = 0;
            if (Int32.TryParse(key, out m))
            {
                if (m > 1)
                {
                    int n = ((ciphertext.Length - 1) / m) + 1;
                    if (n * m >= ciphertext.Length)
                    {
                        string[] rows = new string[m];
                        int emptyCols = n * m - ciphertext.Length;
                        int startEmptyBlock = (n - emptyCols) * m;
                        for (int i = 0; i < ciphertext.Length; i++)
                        {
                            int mod = m;
                            int index = i;
                            if (i / m >= (n - emptyCols))
                            {
                                mod--;
                                index -= startEmptyBlock;
                            }
                            rows[index % mod] += ciphertext[i];
                        }
                        text = String.Join("", rows);

                    }
                    else
                    {
                        throw new Exception("Размер ключа слишком маленький");
                    }

                }
                else
                {
                    throw new Exception("Ключ должен быть положительным числом больше одного");
                }
            }
            else
            {
                throw new Exception("Ключ должен быть положительным числом больше одного");
            }

            return text;
        }

        public override string Encrypt(string text, string key = null)
        {
            string ciphertext = "";
            int m = 0;
            if (Int32.TryParse(key, out m))
            {
                if (m > 1)
                {
                    int n = ((text.Length - 1) / m) + 1;

                    if (n * m >= text.Length)
                    {
                        string[] columns = new string[n];

                        for (int i = 0; i < text.Length; i++)
                        {
                            columns[i % n] += text[i];
                        }
                        ciphertext = String.Join("", columns);

                    }
                    else
                    {
                        throw new Exception("Размер ключа слишком маленький");
                    }

                }
                else
                {
                    throw new Exception("Ключ должен быть положительным числом больше одного");
                }
            }
            else
            {
                throw new Exception("Ключ должен быть положительным числом больше одного");
            }

            return ciphertext;
        }


    }
}
