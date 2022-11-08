using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    public enum NotificationDelayMode
    {
        Delayed,
        WithoutDelay,
        Both
    }
    public class ScheduleNotificationSubSettings : ISettingsProperty
    {
        bool enabled = false;
        int delay = 5;
        bool onlyFirstSchedule = false;
        NotificationDelayMode mode = NotificationDelayMode.Both;
        double volume = 1;

        public bool Enabled
        {
            get => enabled;
            set
            {
                enabled = value;
                PropertyChanged();
            }
        }
        public int Delay
        {
            get => delay;
            set
            {
                delay = value;
                PropertyChanged();
            }
        }
        public bool OnlyFirstSchedule
        {
            get => onlyFirstSchedule;
            set
            {
                onlyFirstSchedule = value;
                PropertyChanged();
            }
        }
        public NotificationDelayMode Mode
        {
            get => mode;
            set
            {
                mode = value;
                PropertyChanged();
            }
        }
        public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                PropertyChanged();
            }
        }

        public event Action? OnPropertyChanged;

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
