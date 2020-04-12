using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace CyptoModule.Views.Tools
{
    public class BoolToSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool input = (bool)value;

            switch (input)
            {
                case true:
                    return Double.NaN;
                default:
                    return 0.0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            double input = (double)value;
            switch (input)
            {
                case Double.NaN:
                    return true;
                default:
                    return false;
            }
        }
    }
}
