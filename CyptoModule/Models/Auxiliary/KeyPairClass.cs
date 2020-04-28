using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CyptoModule.Models.Auxiliary
{
    public class KeyPairClass : BindableBase
    {
        private string _key = "";
        private string _value = "";

        public string Key
        {
            get => _key; 
            set
            {
                _key = value;
                RaisePropertyChanged(Key);
            }
        }
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                RaisePropertyChanged(Value);
            }
        }

        public KeyPairClass(string key, string value)
        {
            Key = key;
            Value = value;
        }
    }
}
