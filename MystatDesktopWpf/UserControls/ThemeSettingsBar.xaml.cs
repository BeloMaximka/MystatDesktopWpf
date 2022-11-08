using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Converters;
using MystatDesktopWpf.Services;
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
using MystatDesktopWpf.ViewModels;
using MystatDesktopWpf.Domain;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ThemeSettings.xaml
    /// </summary>
    public partial class ThemeSettingsBar : UserControl
    {
        public ThemeSettingsBar()
        {
            InitializeComponent();
            DataContext = ThemeSettingsVMSingleton.ViewModel;

        }
        public event Action? OnResetThemeSettings;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ThemeSettingsVMSingleton.ViewModel.IsDarkTheme = true;
            ThemeSettingsVMSingleton.ViewModel.IsColorAdjusted = true;
            ThemeSettingsVMSingleton.ViewModel.ContrastValue = Contrast.Low;
            ThemeSettingsVMSingleton.ViewModel.DesiredContrastRatio = 3.0f;
            ThemeSettingsVMSingleton.ViewModel.ColorSelectionValue = ColorSelection.All;

            OnResetThemeSettings?.Invoke();
        }
    }
}
