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
        readonly Type contentType;
        readonly object? dataContext;

        public MainMenuItem(string nameKey, Type contentType, PackIconKind selectedIcon,
            PackIconKind unselectedIcon, object? dataContext = null)
        {
            this.nameKey = nameKey;
            UpdateName(null, EventArgs.Empty);
            App.LanguageChanged += UpdateName;

            this.contentType = contentType;
            this.dataContext = dataContext;
            SelectedIcon = selectedIcon;
            UnselectedIcon = unselectedIcon;
        }

        void UpdateName(object? sender, EventArgs e)
        {
            Name = (string)App.Current.FindResource(nameKey);
        }

        string nameKey;
        string name;
        public string Name
        {
            get => name;
            set
            {
                name = value;
                OnPropertyChanged(nameof(Name));
            }
        }


        object? content;
        // Чтобы не прогружать все страницы при запуске
        public object? Content => content ??= CreateContent();

        public PackIconKind SelectedIcon { get; set; }
        public PackIconKind UnselectedIcon { get; set; }

        int notifications = 0;
        public object? Notifications
        {
            get
            {
                if (notifications == 0) return null;
                else return notifications < 100 ? notifications : "99+";
            }
        }

        Thickness margin = new(16);
        public Thickness Margin
        {
            get => margin;
            set => SetProperty(ref margin, value);
        }

        object? CreateContent()
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