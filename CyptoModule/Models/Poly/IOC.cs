using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Poly
{
    /// <summary>
    /// Index of Coincidence
    /// </summary>
    public class IOC : PolyAnalysisAbstract
    {
        public override string Name { get => "Метод индекса совпадений"; }

        public override int DoAnalysis(string text, Language language)
        {
            chooseLang(language);

            string clearText = removeUnusedChars(text);
            

            int keySize = calcKeySize(clearText);
            return keySize;
        }

        private int calcKeySize(string text)
        {
            int keySize = 0;

            int max = Math.Min(text.Length, 50);

            double[] keyIC = new double[max];

            for (int i = 1; i < max; i++)
            {
                var arr = getEveryNletter(text, i);
                double ic = 0.0;
                foreach (var item in arr)
                {
                    ic += calcIndex(item);
                }
                ic = ic / arr.Length;
                keyIC[i] = ic;
            }

            double epsilon = 0.01;
            int posibleKeySize = 0;
            double diff = 1;
            for (int i = 1; i < max; i++)
            {
                double temp = Math.Abs(IDEAL - keyIC[i]);
                if ((posibleKeySize == 0) && (temp < epsilon) && (keyIC[i] >= (IDEAL - 0.0053)))
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




        private double calcIndex(string text)
        {
            double index = 0;
            var count = _alphabet.ToDictionary(v => v.ToString(), v => 0);

            text = text.ToUpper();
            foreach (var letter in text)
            {
                count[letter.ToString()]++;
            }

            foreach (var key in count.Keys)
            {
                index += count[key] * (count[key] - 1);
            }

            index = index / (text.Length * (text.Length - 1));

            return index;
        }


        private string[] getEveryNletter(string text, int n)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < n; i++)
            {
                result.Add("");
                for (int j = i; j < text.Length; j+=n)
                {
                    result[i] += text[j];
                }
            }

            return result.ToArray();
        }


    }
}
