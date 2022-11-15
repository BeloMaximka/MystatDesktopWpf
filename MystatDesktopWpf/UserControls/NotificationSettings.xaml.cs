using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for NotificationSettings.xaml
    /// </summary>
    public partial class NotificationSettings : UserControl
    {
        NotificationSettingsViewModel viewModel;
        double savedVolume = 1;

        public NotificationSettings()
        {
            viewModel = new NotificationSettingsViewModel();
            DataContext = viewModel;

            InitializeComponent();
            // Вешаем только сейчас, чтобы не активировать ViewModel
            minutesTextBox.TextChanged += minutesTextBox_TextChanged;

            viewModel.PropertyChanged += ViewModel_PropertyChanged;
            ViewModel_PropertyChanged(null, new PropertyChangedEventArgs("NotificationVolume"));
        }
        private void ViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "NotificationVolume")
            {
                PackIcon icon = (PackIcon)muteButton.Content;
                if (viewModel.NotificationVolume == 0)
                    icon.Kind = PackIconKind.Mute;
                else
                    icon.Kind = PackIconKind.VolumeHigh;
            }
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void minutesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int result;
            if (int.TryParse(minutesTextBox.Text, out result))
                viewModel.NotificationDelay = result;
        }

        private void Button_Mute_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            PackIcon icon = (PackIcon)button.Content;
            if (icon.Kind == PackIconKind.VolumeHigh)
            {
                savedVolume = notificationVolumeSlider.Value;
                notificationVolumeSlider.Value = 0;

                icon.Kind = PackIconKind.Mute;
            }
            else if (icon.Kind == PackIconKind.Mute)
            {
                notificationVolumeSlider.Value = savedVolume;
                icon.Kind = PackIconKind.VolumeHigh;
            }
        }
    }
}
