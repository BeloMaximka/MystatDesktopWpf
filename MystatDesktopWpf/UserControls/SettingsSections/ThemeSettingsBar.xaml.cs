using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Domain;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls.SettingsSections
{
    /// <summary>
    /// Interaction logic for ThemeSettings.xaml
    /// </summary>
    public partial class ThemeSettingsBar : UserControl
    {
        public ThemeSettingsBar()
        {
            InitializeComponent();
        }

        public event Action? OnResetThemeSettings;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsVMSingleton.ViewModel.IsDarkTheme = true;
            ThemeSettingsVMSingleton.ViewModel.IsColorAdjusted = true;
            ThemeSettingsVMSingleton.ViewModel.ContrastValue = Contrast.Low;
            ThemeSettingsVMSingleton.ViewModel.DesiredContrastRatio = 4.5f;
            ThemeSettingsVMSingleton.ViewModel.ColorSelectionValue = ColorSelection.All;

            OnResetThemeSettings?.Invoke();
        }
    }
}
