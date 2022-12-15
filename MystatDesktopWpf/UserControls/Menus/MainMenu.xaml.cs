﻿using MaterialDesignThemes.Wpf.Transitions;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.ViewModels;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for MainMenuUserControl.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        private readonly MainMenuViewModel menuViewModel;
        private readonly HeaderBarViewModel headerViewModel;

        public MainMenu()
        {
            menuViewModel = new();
            this.DataContext = menuViewModel;
            
            InitializeComponent();

            headerViewModel = new();
            headerBar.DataContext = headerViewModel;
        }

        private void Button_Exit_Click(object sender, RoutedEventArgs e)
        {
            // Back to login
            SettingsService.RemoveUserData();
            ScheduleNotificationService.DisableAllNotifications();
            Transitioner.MovePreviousCommand.Execute(null, null);
        }
        private void OnLanguageChange(object sender, RoutedEventArgs e)
        {
            if (sender is not ComboBoxItem item) return;

            var index = int.Parse((string)item.Tag);
            App.Language = App.Languages[index];
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IRefreshable menu = menuViewModel.SelectedItem.Content as IRefreshable;
            menu?.Refresh();
            RefreshButtonDebounce();
        }

        private async void RefreshButtonDebounce()
        {
            refreshButton.IsEnabled = false;
            await Task.Delay(1000);
            refreshButton.IsEnabled = true;
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5 && refreshButton.IsEnabled)
                RefreshButton_Click(null, null);
        }
    }
}
