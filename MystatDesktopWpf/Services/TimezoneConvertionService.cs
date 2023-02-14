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
            var from = SettingsService.Settings.TimezoneConvertion.From;
            var to = SettingsService.Settings.TimezoneConvertion.To;

            if (from is null || to is null)
            {
                return time;
            }

            return TimeZoneInfo.ConvertTime(time, from, to);
        }
        
        public static DateTime Convert(string timeStr)
        {
            var time = DateTime.ParseExact(timeStr, "HH:mm", CultureInfo.InvariantCulture);
            return Convert(time);
        }
    }
}
