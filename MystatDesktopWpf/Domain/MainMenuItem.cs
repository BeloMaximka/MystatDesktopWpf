using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.ViewModels;
using System;
using System.Windows;

namespace MystatDesktopWpf.Domain
{
    public class MainMenuItem : ViewModelBase
    {
        private readonly Type contentType;
        private readonly object? dataContext;

        public MainMenuItem(string nameKey, Type contentType, PackIconKind selectedIcon,
            PackIconKind unselectedIcon, object? dataContext = null)
        {
            this.nameKey = nameKey;
            UpdateName(null, EventArgs.Empty);
            App.LanguageChanged += UpdateName;

            if (dataContext is INotificationCount viewmodel)
                viewmodel.PropertyChanged += Viewmodel_PropertyChanged;

            this.contentType = contentType;
            this.dataContext = dataContext;
            SelectedIcon = selectedIcon;
            UnselectedIcon = unselectedIcon;
        }

        private void Viewmodel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MenuItemNotifications")
                Notifications = ((INotificationCount)sender).MenuItemNotifications;
        }

        private void UpdateName(object? sender, EventArgs e)
        {
            Name = (string)App.Current.FindResource(nameKey);
        }

        private readonly string nameKey;
        private string name;
        public string Name { get => name; set => SetProperty(ref name, value); }

        private object? content;
        // Чтобы не прогружать все страницы при запуске
        public object? Content => content ??= CreateContent();

        public PackIconKind SelectedIcon { get; set; }
        public PackIconKind UnselectedIcon { get; set; }

        private int notifications = 0;
        public object? Notifications
        {
            get
            {
                if (notifications == 0) return null;
                else return notifications < 100 ? notifications : "99+";
            }
            set
            {
                if (value is int count) SetProperty(ref notifications, count);
            }
        }

        private Thickness margin = new(16);
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
    }
}