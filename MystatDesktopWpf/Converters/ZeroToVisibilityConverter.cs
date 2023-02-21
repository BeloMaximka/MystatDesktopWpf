using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MystatDesktopWpf.Converters
{
    internal class ZeroToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (double.TryParse(value.ToString(), out double number))
            {
                return number == 0 ? Visibility.Visible : Visibility.Collapsed;
            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
