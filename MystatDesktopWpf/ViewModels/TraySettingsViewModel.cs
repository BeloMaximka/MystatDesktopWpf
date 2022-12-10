using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;

namespace MystatDesktopWpf.ViewModels
{
    internal class TraySettingsViewModel : ViewModelBase
    {
        private readonly TraySubSettings traySettings;

        public TraySettingsViewModel()
        {
            traySettings = SettingsService.Settings.Tray;
        }

        public bool IsAlwaysTray
        {
            get => traySettings.TrayBehavior == TrayBehavior.AlwaysMove;
            set
            {
                if (value)
                {
                    OnPropertyChanged(nameof(IsAlwaysTray));
                    traySettings.TrayBehavior = TrayBehavior.AlwaysMove;
                }
            }
        }

        public bool IsCloseTray
        {
            get => traySettings.TrayBehavior == TrayBehavior.OnlyOnClose;
            set
            {
                if (value)
                {
                    OnPropertyChanged(nameof(IsCloseTray));
                    traySettings.TrayBehavior = TrayBehavior.OnlyOnClose;
                }
            }
        }

        public bool IsNoTray
        {
            get => traySettings.TrayBehavior == TrayBehavior.NeverMove;
            set
            {
                if (value)
                {
                    OnPropertyChanged(nameof(IsNoTray));
                    traySettings.TrayBehavior = TrayBehavior.NeverMove;
                }
            }
        }

    }
}
