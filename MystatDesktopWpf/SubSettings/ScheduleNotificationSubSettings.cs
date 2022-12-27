using MystatDesktopWpf.Services;
using System;

namespace MystatDesktopWpf.SubSettings
{
    public enum NotificationDelayMode
    {
        Delayed,
        WithoutDelay,
        Both
    }
    public class ScheduleNotificationSubSettings : ISettingsProperty
    {
        private bool enabled = false;
        private int delay = 5;
        private bool onlyFirstSchedule = false;
        private NotificationDelayMode mode = NotificationDelayMode.Both;
        private double volume = 1;

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
