using MaterialDesignThemes.Wpf;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using System;
using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.Domain
{
    public class MainMenuItem : ViewModelBase
    {
        private readonly Type contentType;
        private readonly object? dataContext;

        private object? content;
        private Thickness margin = new(16);

        private int notifications = 0;

        public MainMenuItem(string name, Type contentType, PackIconKind selectedIcon,
            PackIconKind unselectedIcon, object? dataContext = null)
        {
            Name = name;
            this.contentType = contentType;
            this.dataContext = dataContext;
            SelectedIcon = selectedIcon;
            UnselectedIcon = unselectedIcon;
        }

        public string Name { get; }

        // Not to initialize all controls on startup
        public object? Content => content ??= CreateContent();

        public PackIconKind SelectedIcon { get; set; }
        public PackIconKind UnselectedIcon { get; set; }

        public object? Notifications
        {
            get
            {
                if (notifications == 0) return null;
                else return notifications < 100 ? notifications : "99+";
            }
        }

        public Thickness Margin
        {
            get => margin;
            set => SetProperty(ref margin, value);
        }

        private object? CreateContent()
        {
            var content = Activator.CreateInstance(contentType);
            if (dataContext != null && content is FrameworkElement element)
            {
                element.DataContext = dataContext;
            }

            return content;
        }

        public void AddNewNotification()
        {
            notifications++;
            OnPropertyChanged(nameof(Notifications));
        }

        public void DismissAllNotifications()
        {
            notifications = 0;
            OnPropertyChanged(nameof(Notifications));
        }
    }
}