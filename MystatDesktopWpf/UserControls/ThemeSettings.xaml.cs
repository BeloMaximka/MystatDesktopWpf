using MystatDesktopWpf.Converters;
using MystatDesktopWpf.ViewModels;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ThemeSettings.xaml
    /// </summary>
    public partial class ThemeSettings : UserControl
    {
        private readonly ThemeColorViewModel viewModel = new();
        public ThemeSettings()
        {
            InitializeComponent();
            DataContext = viewModel;
            settingsBar.OnResetThemeSettings += ResetTheme;
        }

        private void ResetTheme()
        {

            viewModel.SelectedColor = ColorToHexConverter.ConvertBack("#FF673AB7");
        }
    }
}
