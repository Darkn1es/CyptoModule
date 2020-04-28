using CyptoModule.Models;
using CyptoModule.Models.Auxiliary;
using LiveCharts;
using LiveCharts.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace CyptoModule.ViewModels
{
    public class FreqAnalysisVM : BindableBase
    {
        private FreqAnalysis _freqAnalysis;
        private string _text = "";
        private string _language = "Русский";
        public string Language
        {
            get => _language;
            set
            {
                _language = value;
                switch (_language)
                {
                    case "Русский":
                        _freqAnalysis.ChangeLanguage(FreqAnalysis.Language.Russian);
                        break;
                    case "English":
                        _freqAnalysis.ChangeLanguage(FreqAnalysis.Language.English);
                        break;
                    default:
                        break;
                }
                updateHistogram();
                RaisePropertyChanged(nameof(Language));
            }
        }

        private bool _isFixed = false;
        public bool IsFixed
        {
            get => _isFixed;
            set
            {
                _isFixed = value;
                if (_isFixed == true)
                {
                    _freqAnalysis.Text = Text;

                    /*Histogram[0].Values.Clear();
                    Histogram[1].Values.Clear();

                    foreach (var key in _freqAnalysis.CurrentFreq.Keys)
                    {
                        Histogram[0].Values.Add(_freqAnalysis.CurrentFreq[key]);
                        string replacedKey = _freqAnalysis.ReplaceRule[key];
                        Histogram[1].Values.Add(_freqAnalysis.CurrentAlphabet[replacedKey]);
                    }*/
                    updateHistogram();
                }

                RaisePropertyChanged(nameof(IsFixed));
            }
        }

        public ObservableCollection<KeyPairClass> ReplaceRule { get; set; } = new ObservableCollection<KeyPairClass>();

        public FreqAnalysisVM()
        {
            ReplaceRule = new ObservableCollection<KeyPairClass>();
            _freqAnalysis = new FreqAnalysis();
            _freqAnalysis.PropertyChanged += _freqAnalysis_PropertyChanged;
            ReplaceRule.CollectionChanged += ReplaceRule_CollectionChanged;
            Histogram = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "В тексте",
                    Values = new ChartValues<double>(_freqAnalysis.CurrentFreq.Values.ToArray())
                }
            };

            //adding series will update and animate the chart automatically 
            Histogram.Add(new ColumnSeries
            {
                Title = "Эмпирическая",
                Values = new ChartValues<double>(_freqAnalysis.CurrentAlphabet.Values.ToArray()),
                LabelPoint = t =>
                {
                    var currentLetter = _freqAnalysis.CurrentAlphabet.ElementAt(t.Key);
                    var replacedLetter = _freqAnalysis.ReplaceRule[currentLetter.Key];



                    return _freqAnalysis.CurrentAlphabet[replacedLetter].ToString("N") + "% - " + replacedLetter;
                }
            }); 
            //also adding values updates and animates the chart automatically

            Labels = _freqAnalysis.CurrentAlphabet.Keys.ToArray();
            Formatter = value => value.ToString("N") + "%";



            OptimizeCommand = new DelegateCommand(() =>
            {
                _freqAnalysis.OptimizeRule();
                ReplaceRule.Clear();
                foreach (var item in _freqAnalysis.ReplaceRule)
                {
                    ReplaceRule.Add(new KeyPairClass(item.Key, item.Value));
                }
                Text = _freqAnalysis.GetTextWithRule();
                
                Histogram[1].Values.Clear();
                foreach (var key in _freqAnalysis.CurrentFreq.Keys)
                {
                    string replacedKey = _freqAnalysis.ReplaceRule[key];
                    Histogram[1].Values.Add(_freqAnalysis.CurrentAlphabet[replacedKey]);
                }
            });

            ReplaceRule.Clear();
            foreach (var item in _freqAnalysis.ReplaceRule)
            {
                //ReplaceRule.Add(new ObservableCollection<string>() { item.Key, item.Value });
                ReplaceRule.Add(new KeyPairClass(item.Key, item.Value));
            }
            //RaisePropertyChanged(nameof(ReplaceRule));
        }

        private void ReplaceRule_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (KeyPairClass item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;
                    

            if (e.OldItems != null)
                foreach (KeyPairClass item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var a = sender as KeyPairClass;
            if ((a?.Value == null) || (a?.Key == null))
            {
                return;
            }
            var newKP = _freqAnalysis.ApplyRule(a.Key, a.Value);
            var index = ReplaceRule.IndexOf( ReplaceRule.First(t => t.Key == newKP.Key));
            ReplaceRule.RemoveAt(index);
            ReplaceRule.Insert(index, newKP);
            Text = _freqAnalysis.GetTextWithRule();
            Histogram[1].Values.Clear();
            foreach (var key in _freqAnalysis.CurrentFreq.Keys)
            {
                string replacedKey = _freqAnalysis.ReplaceRule[key];
                Histogram[1].Values.Add(_freqAnalysis.CurrentAlphabet[replacedKey]);
            }
        }

        private void _freqAnalysis_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
             RaisePropertyChanged(e.PropertyName);
        }

        public float[] ChartValues { get; set; }

        
        public SeriesCollection Histogram { get; set; }
        public string[] Labels { get; set; }
        public Func<double, string> Formatter { get; set; }

        public string Text
        {
            get => _text;
            set
            {
                _text = value;
                RaisePropertyChanged(nameof(Text));
            }
        }

        public DelegateCommand TestCommand { get; }
        public DelegateCommand OptimizeCommand { get; }

        private void updateHistogram()
        {
            Labels = _freqAnalysis.CurrentAlphabet.Keys.ToArray();
            RaisePropertyChanged(nameof(Labels));
            ReplaceRule.Clear();
            foreach (var item in _freqAnalysis.ReplaceRule)
            {
                ReplaceRule.Add(new KeyPairClass(item.Key, item.Value));
            }

            Histogram[0].Values.Clear();
            Histogram[1].Values.Clear();

            foreach (var key in _freqAnalysis.CurrentFreq.Keys)
            {
                Histogram[0].Values.Add(_freqAnalysis.CurrentFreq[key]);
                string replacedKey = _freqAnalysis.ReplaceRule[key];
                Histogram[1].Values.Add(_freqAnalysis.CurrentAlphabet[replacedKey]);
            }


        }

    }
}
