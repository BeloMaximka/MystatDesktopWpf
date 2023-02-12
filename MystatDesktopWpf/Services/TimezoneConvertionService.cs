using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Services
{
    internal static class TimezoneConvertionService
    {
        public static DateTime Convert(DateTime time)
        {
            return TimeZoneInfo.ConvertTime(time, SettingsService.Settings.TimezoneConvertion.From, SettingsService.Settings.TimezoneConvertion.To);
        }
        
        public static DateTime Convert(string timeStr)
        {
            var time = DateTime.ParseExact(timeStr, "HH:mm", CultureInfo.InvariantCulture);
            return Convert(time);
        }
    }
}
