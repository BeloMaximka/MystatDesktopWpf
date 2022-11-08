using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace MystatDesktopWpf.Converters
{
    [ValueConversion(typeof(Color), typeof(string))]
    public class ColorToHexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Color c)
            {
                return $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";
            }
            return Binding.DoNothing;
        }
        public static string Convert(Color c) => $"#{c.A:X2}{c.R:X2}{c.G:X2}{c.B:X2}";

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string hex)
            {
                return (Color)ColorConverter.ConvertFromString(hex);
            }
            return default(Color);
        }
        public static Color ConvertBack(string hex) => (Color)ColorConverter.ConvertFromString(hex);
    }
}