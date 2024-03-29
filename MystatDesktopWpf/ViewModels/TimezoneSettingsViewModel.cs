﻿using MystatDesktopWpf.Services;
using MystatDesktopWpf.SubSettings;
using System;
using System.Collections.ObjectModel;

namespace MystatDesktopWpf.ViewModels
{
    internal class TimezoneSettingsViewModel : ViewModelBase
    {
        private readonly TimezoneSubSettings timezoneSubSettings;

        public ReadOnlyCollection<TimeZoneInfo> TimeZones { get; set; }

        public TimeZoneInfo TimezoneFrom
        {
            get => timezoneSubSettings.From;
            set
            {
                timezoneSubSettings.From = value;
                OnPropertyChanged(nameof(TimezoneFrom));
            }
        }
        public TimeZoneInfo TimezoneTo
        {
            get => timezoneSubSettings.To;
            set
            {
                timezoneSubSettings.To = value;
                OnPropertyChanged(nameof(TimezoneTo));
            }
        }

        public TimezoneSettingsViewModel()
        {
            TimeZones = TimeZoneInfo.GetSystemTimeZones();
            timezoneSubSettings = SettingsService.Settings.TimezoneConvertion;
        }
    }
}
