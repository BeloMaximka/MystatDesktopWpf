using MystatDesktopWpf.Services;
using System;

namespace MystatDesktopWpf.Domain
{
    internal enum TrayBehavior
    {
        AlwaysMove,
        OnlyOnClose,
        NeverMove
    }

    internal class TraySubSettings : ISettingsProperty
    {
        public event Action? OnPropertyChanged;

        private TrayBehavior trayBehavior = TrayBehavior.AlwaysMove;

        public TrayBehavior TrayBehavior
        {
            get => trayBehavior;
            set
            {
                trayBehavior = value;
                PropertyChanged();
            }
        }

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
