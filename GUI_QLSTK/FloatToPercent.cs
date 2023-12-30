using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GUI_QLSTK
{
    class FloatToPercent : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (double)value;
            return input.ToString("P3");
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var input = (string)value;

            input = input.Replace("%", "");
            if (!double.TryParse(input, out double result))
            {
                return -1;
            }
            return result / 100;
        }
    }
}
