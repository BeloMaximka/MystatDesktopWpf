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

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ThemeSettings.xaml
    /// </summary>
    public partial class ThemeSettingsBar : UserControl
    {
        ThemeSettingsViewModel viewModel = new();
        public ThemeSettingsBar()
        {
            InitializeComponent();
            DataContext = viewModel;

        }
        public event Action? OnResetThemeSettings;
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            viewModel.IsDarkTheme = true;
            viewModel.IsColorAdjusted = true;
            viewModel.ContrastValue = Contrast.Low;
            viewModel.DesiredContrastRatio = 3.0f;
            viewModel.ColorSelectionValue = ColorSelection.All;

            OnResetThemeSettings?.Invoke();
        }
    }
}
