using CyptoModule.Models.Ciphers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Interfaces
{
    public abstract class PolyAnalysisAbstract
    {
        public abstract string Name { get; }
        public abstract int DoAnalysis(string text, Language language);

        protected const string _rusAlphabet = "АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯ";
        protected const string _engAlphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        protected string _alphabet = "";
        protected Dictionary<string, double> _alphabetDic;
        protected double IDEAL;

        protected readonly Dictionary<string, double> _rusAlphabetDic = new Dictionary<string, double>()
        {
            {"А", 8.01}, {"Б", 1.59}, {"В", 4.54}, {"Г", 1.70}, {"Д", 2.98},
            {"Е", 8.45}, {"Ё", 0.04}, {"Ж", 0.94}, {"З", 1.65}, {"И", 7.35},
            {"Й", 1.21}, {"К", 3.49}, {"Л", 4.40}, {"М", 3.21}, {"Н", 6.70},
            {"О", 10.97}, {"П", 2.81}, {"Р", 4.73}, {"С", 5.47}, {"Т", 6.26},
            {"У", 2.62}, {"Ф", 0.26}, {"Х", 0.97}, {"Ц", 0.48}, {"Ч", 1.44},
            {"Ш", 0.73}, {"Щ", 0.36}, {"Ъ", 0.04}, {"Ы", 1.90}, {"Ь", 1.74},
            {"Э", 0.32}, {"Ю", 0.64}, {"Я", 2.01}
        };

        protected readonly Dictionary<string, double> _engAlphabetDic = new Dictionary<string, double>()
        {
            {"A", 8.497}, {"B", 1.492}, {"C", 2.202}, {"D", 4.253}, {"E", 11.162},
            {"F", 2.228}, {"G", 2.015}, {"H", 6.094}, {"I", 7.546}, {"J", 0.153},
            {"K", 1.292}, {"L", 4.025}, {"M", 2.406}, {"N", 6.749}, {"O", 7.507},
            {"P", 1.929}, {"Q", 0.095}, {"R", 7.587}, {"S", 6.327}, {"T", 9.356},
            {"U", 2.758}, {"V", 0.978}, {"W", 2.560}, {"X", 0.150}, {"Y", 1.994},
            {"Z", 0.077}
        };


        protected string removeUnusedChars(string text)
        {
            string tempText = "";
            text = text.ToUpper();

            foreach (var letter in text)
            {
                if (_alphabet.Contains(letter.ToString()))
                {
                    tempText += letter;
                }
            }
            return tempText;
        }

        protected void chooseLang(Language language)
        {
            if (language == Language.RUSSIAN)
            {
                IDEAL = 0.0553;
                _alphabet = _rusAlphabet;
                _alphabetDic = _rusAlphabetDic;
            }
            else if (language == Language.ENGLISH)
            {
                IDEAL = 0.0644;
                _alphabet = _engAlphabet;
                _alphabetDic = _engAlphabetDic;

            }
        }
        public enum Language
        {
            RUSSIAN,
            ENGLISH
        }

        public string PredictKey(string text, int keySize, Language language)
        {
            chooseLang(language);
            string clearText = removeUnusedChars(text);
            string result = "";

            double[] freqs = _alphabetDic.Values.ToArray();

            for (int i = 0; i < keySize; i++)
            {
                string sequence = "";

                for (int j = 0; j < clearText.Length; j++)
                {
                    if (j % keySize == i)
                    {
                        sequence += clearText[j];
                    }
                }

                result += unnamedFunc(sequence, freqs);


            }


            return result;

        }

        private string unnamedFunc(string sequence, double[] freqs)
        {
            double[] allChiQuadrats = new double[_alphabet.Length];
            Caesar ceaser = new Caesar();

            for (int i = 0; i < _alphabet.Length; i++)
            {
                string temp = sequence;
                double tempChiSum = 0.0;

                var sequenceOffset = ceaser.Decrypt(temp, i.ToString()).ToList();

                var currentFreq = _alphabetDic.ToDictionary(t => t.Key, t => 0.0);
                var lettersCount = _alphabetDic.ToDictionary(t => t.Key, t => 0);

                foreach (var letter in sequenceOffset)
                {   
                    lettersCount[letter.ToString()]++;
                }

                foreach (var key in lettersCount.Keys)
                {
                    currentFreq[key] = Convert.ToDouble(lettersCount[key]) / sequenceOffset.Count;
                }

                foreach (var key in currentFreq.Keys)
                {
                    double tableFreq = _alphabetDic[key] / 100;
                    tempChiSum += Math.Pow(currentFreq[key] - tableFreq, 2) / tableFreq;
                }
                allChiQuadrats[i] = tempChiSum;
            }

            int shift = 0;
            double min = double.MaxValue;

            for (int i = 0; i < allChiQuadrats.Length; i++)
            {
                if (allChiQuadrats[i] < min)
                {
                    min = allChiQuadrats[i];
                    shift = i;
                }
            }

            return _alphabet[shift].ToString();


        }
    }
}
