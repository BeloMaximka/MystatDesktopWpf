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
        public MainMenu()
        {
            this.DataContext = new MainMenuViewModel();
            InitializeComponent();

            darkModeButton.DataContext = ThemeSettingsVMSingleton.ViewModel;
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
    }
}
