using System;
using System.Globalization;
using System.Windows.Data;

namespace MystatDesktopWpf.Converters
{
    internal class FormatDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is string text && DateTime.TryParse(text, out DateTime result))
            {
                return result.ToString((string)parameter);
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
