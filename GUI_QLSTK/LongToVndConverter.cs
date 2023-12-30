using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GUI_QLSTK
{
    class LongToVndConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (long)value;
            return input.ToString("C0", CultureInfo.CreateSpecificCulture("vi-VN"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (string)value;
            input = input.Replace("₫", "").Replace(",", "");
            if (!long.TryParse(input, out long result))
            {
                return -1;
            }
            return result;
        }
    }
}

