using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Poly
{
    public class Autocorrelation : PolyAnalysisAbstract
    {
        public override string Name { get => "Автокорреляционный метод"; }

        public override int DoAnalysis(string text, Language language)
        {
            chooseLang(language);

            string clearText = removeUnusedChars(text);
            int keySize = 0;

            int max = 50 < clearText.Length ? 50 : clearText.Length - 1;

            string[] texts = new string[max];
            double[] indexes = new double[max];

            texts[0] = clearText;
            for (int i = 1; i < max; i++)
            {
                texts[i] = shiftString(texts[i - 1]);

                indexes[i] = 0.0;
                int countOfDub = 0;
                for (int j = 0; j < clearText.Length; j++)
                {
                    if (texts[i][j] == '\0')
                    {
                        continue;
                    }
                    if (texts[i][j] == texts[0][j])
                    {
                        countOfDub++;
                    }
                }
                indexes[i] = Convert.ToDouble(countOfDub) / Convert.ToDouble(clearText.Length - i);
            }

            double epsilon = 0.015;
            int posibleKeySize = 0;
            double diff = 1;
            for (int i = 1; i < max; i++)
            {
                double temp = Math.Abs(IDEAL - indexes[i]);
                if ((posibleKeySize == 0) && (temp < epsilon) && (indexes[i] >= (IDEAL - 0.0053)))
                {
                    posibleKeySize = i;
                }
                if (temp < diff)
                {
                    diff = temp;
                    keySize = i;
                }
            }

            if (posibleKeySize != 0)
            {
                return posibleKeySize;
            }

            return keySize;

        }

        private string shiftString(string text)
        {
            return "\0" + text.Substring(0, text.Length - 1);
        }
    }
}
