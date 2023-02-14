using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MystatDesktopWpf.Converters
{
    internal class TimeZoneToNameConverter : IValueConverter
    {
        private static IDictionary<string, string> timezones;
        static TimeZoneToNameConverter()
        {
            timezones = TimeZoneNames.TZNames.GetDisplayNames(App.Language.Name);
            App.LanguageChanged += App_LanguageChanged;
        }

        private static void App_LanguageChanged(object? sender, EventArgs e)
        {
            timezones = TimeZoneNames.TZNames.GetDisplayNames(App.Language.Name);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TimeZoneInfo info)
            {
                if (timezones.TryGetValue(info.Id, out string? result))
                {
                    return result;
                }
                else
                {
                    return info.DisplayName;
                }

            }
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}