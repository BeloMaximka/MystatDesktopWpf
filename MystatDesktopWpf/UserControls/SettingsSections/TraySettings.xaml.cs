using MystatDesktopWpf.ViewModels;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for TraySettings.xaml
    /// </summary>
    public partial class TraySettings : UserControl
    {
        private readonly TraySettingsViewModel viewModel;

        public TraySettings()
        {
            viewModel = new();
            DataContext = viewModel;

            InitializeComponent();
        }
    }
}
