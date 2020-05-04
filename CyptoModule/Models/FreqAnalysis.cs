using CyptoModule.Models.Auxiliary;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models
{
    public class FreqAnalysis : BindableBase
    {

        private string _text = "";
        private StringBuilder _processText;
        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                _processText = new StringBuilder(_text);
                initArrays();
                calcFreq();
            }
        }

        private readonly  Dictionary<string, double> _rusAlphabet = new Dictionary<string, double>()
        {
            {"А", 8.01}, {"Б", 1.59}, {"В", 4.54}, {"Г", 1.70}, {"Д", 2.98},
            {"Е", 8.45}, {"Ё", 0.04}, {"Ж", 0.94}, {"З", 1.65}, {"И", 7.35},
            {"Й", 1.21}, {"К", 3.49}, {"Л", 4.40}, {"М", 3.21}, {"Н", 6.70},
            {"О", 10.97}, {"П", 2.81}, {"Р", 4.73}, {"С", 5.47}, {"Т", 6.26},
            {"У", 2.62}, {"Ф", 0.26}, {"Х", 0.97}, {"Ц", 0.48}, {"Ч", 1.44},
            {"Ш", 0.73}, {"Щ", 0.36}, {"Ъ", 0.04}, {"Ы", 1.90}, {"Ь", 1.74},
            {"Э", 0.32}, {"Ю", 0.64}, {"Я", 2.01}
        };
        private readonly Dictionary<string, double> _engAlphabet = new Dictionary<string, double>()
        {
            {"A", 8.497}, {"B", 1.492}, {"C", 2.202}, {"D", 4.253}, {"E", 11.162},
            {"F", 2.228}, {"G", 2.015}, {"H", 6.094}, {"I", 7.546}, {"J", 0.153},
            {"K", 1.292}, {"L", 4.025}, {"M", 2.406}, {"N", 6.749}, {"O", 7.507},
            {"P", 1.929}, {"Q", 0.095}, {"R", 7.587}, {"S", 6.327}, {"T", 9.356},
            {"U", 2.758}, {"V", 0.978}, {"W", 2.560}, {"X", 0.150}, {"Y", 1.994},
            {"Z", 0.077}
        };

        public Dictionary<string, double> CurrentAlphabet;
        public Dictionary<string, double> CurrentFreq;
        public Dictionary<string, string> ReplaceRule;

        private Dictionary<string, List<int>> _lettersIndex;
        public FreqAnalysis()
        {
            CurrentAlphabet = _rusAlphabet;
            initArrays();
        }

        public void ChangeLanguage(Language language)
        {
            switch (language)
            {
                case Language.Russian:
                    CurrentAlphabet = _rusAlphabet;
                    break;
                case Language.English:
                    CurrentAlphabet = _engAlphabet;
                    break;
                default:
                    break;
            }
            initArrays();
        }

        public KeyPairClass ApplyRule(string key, string newValue)
        {
            var temp = ReplaceRule.First((t) => t.Key == key);

            string temp1 = temp.Value;

            string key2 = ReplaceRule.FirstOrDefault(x => x.Value == newValue).Key;

            ReplaceRule[key] = newValue;
            ReplaceRule[key2] = temp1;

            return new KeyPairClass(key2, ReplaceRule[key2]);


        }

        public void OptimizeRule()
        {
            var freqMainList = CurrentAlphabet.ToList();
            var freqTextList = CurrentFreq.ToList();

            freqMainList.Sort((a, b) => a.Value.CompareTo(b.Value));
            freqTextList.Sort((a, b) => a.Value.CompareTo(b.Value));

            for (int i = 0; i < freqMainList.Count; i++)
            {
                ReplaceRule[freqTextList[i].Key] = freqMainList[i].Key;
            }
        }

        public string GetTextWithRule()
        {
            foreach (var key in _lettersIndex.Keys)
            {
                foreach (var index in _lettersIndex[key])
                {
                    _processText[index] = char.IsUpper(_processText[index]) ? ReplaceRule[key][0] : char.ToLower(ReplaceRule[key][0]);
                }
            }

            return _processText.ToString();
        }

        private void calcFreq()
        {
            string upperText = Text.ToUpper();
            int size = 0;
            for (int i = 0; i < upperText.Length; i++)
            {
                string letter = upperText[i].ToString();
                if (CurrentAlphabet.ContainsKey(letter))
                {
                    _lettersIndex[letter].Add(i);
                    size++;
                }
            }
            foreach (var key in CurrentAlphabet.Keys)
            {
                double countOfLetter = _lettersIndex[key].Count;
                double freq = (countOfLetter / size) * 100;
                if (double.IsNaN( freq))
                {
                    freq = 0;
                }
                CurrentFreq[key] = freq;
            }
        }

        private void initArrays()
        {
            CurrentFreq = new Dictionary<string, double>();
            ReplaceRule = new Dictionary<string, string>();
            _lettersIndex = new Dictionary<string, List<int>>();

            foreach (var key in CurrentAlphabet.Keys)
            {
                CurrentFreq.Add(key, 0);
                _lettersIndex.Add(key, new List<int>());
                ReplaceRule.Add(key, key);
            }
        }

        public enum Language
        {
            Russian,
            English
        }

    }
}
