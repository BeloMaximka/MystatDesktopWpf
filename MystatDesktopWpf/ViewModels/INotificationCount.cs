using System.ComponentModel;

namespace MystatDesktopWpf.ViewModels
{
    internal interface INotificationCount : INotifyPropertyChanged
    {
        public int MenuItemNotifications { get; }
    }
}
