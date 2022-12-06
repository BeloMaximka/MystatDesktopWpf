using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for MainMenuUserControl.xaml
    /// </summary>
    public partial class MainMenu : UserControl
    {
        MainMenuViewModel viewModel;
        public MainMenu()
        {
            viewModel = new MainMenuViewModel();
            this.DataContext = viewModel;
            InitializeComponent();
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
            var item = sender as ComboBoxItem;

            if (item is null) return;

            var index = int.Parse(item.Tag as string);
            App.Language = App.Languages[index];
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            IRefreshable menu = viewModel.SelectedItem.Content as IRefreshable;
            menu?.Refresh();
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.F5)
                RefreshButton_Click(null, null);
        }
    }
}
