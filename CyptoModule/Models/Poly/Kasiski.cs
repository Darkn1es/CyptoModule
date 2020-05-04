using CyptoModule.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Poly
{
    public class Kasiski : PolyAnalysisAbstract
    {
        public override string Name { get => "Метод Касиски"; }

        public override int DoAnalysis(string text, Language language)
        {
            chooseLang(language);

            string clearText = removeUnusedChars(text);
            
            Dictionary<string, List<int>> threegrams = new Dictionary<string, List<int>>();

            for (int i = 0; i < (clearText.Length - 3); i++)
            {
                string threegram = clearText.Substring(i, 3);
                if (threegrams.ContainsKey(threegram))
                {
                    continue;
                }
                List<int> indexes = new List<int>();

                for (int j = (i + 3); j < clearText.Length;)
                {
                    int index = clearText.IndexOf(threegram, j);
                    if (index == -1)
                    {
                        break;
                    }
                    indexes.Add(index);
                    j += index + 3;
                }

                if (indexes.Count > 1)
                {
                    threegrams.Add(threegram, indexes);
                }
            }

            var gcdThreegrams = threegrams.ToDictionary(t => t.Key, t => 0);

            foreach (var key in threegrams.Keys)
            {
                var indexes = threegrams[key];

                List<int> distances = new List<int>();
                for (int i = 1; i < indexes.Count; i++)
                {
                    int distance = Math.Abs(indexes[i-1] - indexes[i]);
                    distances.Add(distance);
                }
                int gcd = calcGCD(distances);
                gcdThreegrams[key] = gcd;
            }
            var values = gcdThreegrams.Values.ToList();

            // fix error


            if (values.Count > 2)
            {
                Dictionary<int, int> outOfFantasy = new Dictionary<int, int>();

                for (int i = 0; i < (values.Count - 3); i++)
                {
                    int gcd = calcGCD(values.GetRange(i, 3).ToList());
                    if (outOfFantasy.ContainsKey(gcd))
                    {
                        outOfFantasy[gcd]++;
                    }
                    else
                    {
                        if (gcd == 1)
                        {
                            continue;
                        }
                        outOfFantasy.Add(gcd, 1);
                    }
                }

                return outOfFantasy.Aggregate((l, r) => l.Value > r.Value ? l : r).Key;
            }

            return calcGCD(gcdThreegrams.Values.ToList());

        } 

        private int calcGCD(int a, int b)
        {
            while (a != 0 && b != 0)
            {
                if (a > b)
                    a %= b;
                else
                    b %= a;
            }

            return a == 0 ? b : a;
        }

        private int calcGCD(List<int> array)
        {
            if (array.Count == 0)
            {
                throw new Exception("Array is empty");
            }
            if (array.Count == 1)
            {
                return array[0];
            }
            int gcd = array[0];
            for (int i = 1; i < array.Count; i++)
            {
                gcd = calcGCD(array[i], gcd);
            }
            return gcd;
        }
    }
}
