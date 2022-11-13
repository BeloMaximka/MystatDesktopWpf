using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    enum TrayBehavior
    {
        AlwaysMove,
        OnlyOnClose,
        NeverMove
    }

    internal class TraySubSettings : ISettingsProperty
    {
        public event Action? OnPropertyChanged;

        TrayBehavior trayBehavior = TrayBehavior.AlwaysMove;

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
