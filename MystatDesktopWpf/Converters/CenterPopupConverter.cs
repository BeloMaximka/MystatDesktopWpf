using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows;

namespace MystatDesktopWpf.Converters
{
    public class CenterPopupConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.FirstOrDefault(v => v == DependencyProperty.UnsetValue) != null)
                return double.NaN;
            return ((double)values[1] - (double)values[0]) / -2.0;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
