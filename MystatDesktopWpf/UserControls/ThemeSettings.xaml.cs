using MystatDesktopWpf.Converters;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.ViewModels;
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

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ThemeSettings.xaml
    /// </summary>
    public partial class ThemeSettings : UserControl
    {
        ThemeColorViewModel viewModel = new();
        public ThemeSettings()
        {
            InitializeComponent();
            DataContext = viewModel;
            settingsBar.OnResetThemeSettings += ResetTheme;
        }

        void ResetTheme()
        {
            
            viewModel.SelectedColor= ColorToHexConverter.ConvertBack("#FF673AB7");
        }
    }
}
