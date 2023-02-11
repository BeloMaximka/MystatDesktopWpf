using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.SubSettings
{
    internal class TimezoneSubSettings : ISettingsProperty
    {
        public event Action? OnPropertyChanged;

        TimeZoneInfo from = TimeZoneInfo.Local;
        TimeZoneInfo to = TimeZoneInfo.Local;

        public TimeZoneInfo From
        {
            get => from;
            set
            {
                from = value;
                PropertyChanged();
            }
        }
        public TimeZoneInfo To
        {
            get => to;
            set
            {
                to = value;
                PropertyChanged();
            }
        }

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
