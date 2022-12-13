using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace MystatDesktopWpf.Converters
{
    [ValueConversion(typeof(bool), typeof(bool))]
    internal class PopupIsOpenConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.Any(value => value is bool boolean && boolean);
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
