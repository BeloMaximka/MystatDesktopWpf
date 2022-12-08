using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.ViewModels;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for NotificationSettings.xaml
    /// </summary>
    public partial class NotificationSettings : UserControl
    {
        private readonly NotificationSettingsViewModel viewModel;
        private double savedVolume = 1;

        public NotificationSettings()
        {
            viewModel = new NotificationSettingsViewModel();
            DataContext = viewModel;

            InitializeComponent();
            // Вешаем только сейчас, чтобы не активировать ViewModel
            minutesTextBox.TextChanged += MinutesTextBox_TextChanged;

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
            Regex regex = new("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void MinutesTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(minutesTextBox.Text, out int result))
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
