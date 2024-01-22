using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MystatDesktopWpf.UserControls.SettingsSections
{
    /// <summary>
    /// Interaction logic for ExperimentalSettings.xaml
    /// </summary>
    public partial class ExperimentalSettings : UserControl
    {
        public ExperimentalSettings()
        {
            InitializeComponent();
            BypassUploadToggle.IsChecked = SettingsService.Settings.Experimental.BypassUploadRestrictions;
        }

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            MystatAPISingleton.Client.BypassUploadRestrictions = true;
            SettingsService.Settings.Experimental.BypassUploadRestrictions = true;
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            MystatAPISingleton.Client.BypassUploadRestrictions = false;
            SettingsService.Settings.Experimental.BypassUploadRestrictions = false;
        }
    }
}
