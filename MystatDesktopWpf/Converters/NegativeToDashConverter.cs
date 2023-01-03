using System;
using System.Globalization;
using System.Windows.Data;

namespace MystatDesktopWpf.Converters
{
    internal class NegativeToDashConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int && (int)value >= 0) return value.ToString() + parameter?.ToString();
            return "-";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
