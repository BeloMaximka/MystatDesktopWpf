using MystatDesktopWpf.ViewModels;
using System;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for TimezoneSettings.xaml
    /// </summary>
    public partial class TimezoneSettings : UserControl
    {
        private readonly TimezoneSettingsViewModel viewModel;

        public TimezoneSettings()
        {
            viewModel = new();
            DataContext = viewModel;

            InitializeComponent();

            App.LanguageChanged += App_LanguageChanged;

        }


        // Update ComboBox list and selected item display names
        private void App_LanguageChanged(object? sender, EventArgs e)
        {
            var temp = FromComboBox.SelectedIndex;
            FromComboBox.SelectedIndex = -1;
            FromComboBox.Items.Refresh();
            FromComboBox.SelectedIndex = temp;

            temp = ToComboBox.SelectedIndex;
            ToComboBox.SelectedIndex = -1;
            ToComboBox.Items.Refresh();
            ToComboBox.SelectedIndex = temp;
        }
    }
}
