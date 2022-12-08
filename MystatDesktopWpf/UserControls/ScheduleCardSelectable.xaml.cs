using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for ScheduleCardDialog.xaml
    /// </summary>
    public partial class ScheduleCardSelectable : UserControl
    {
        public DialogHost DialogHost { set => closeButton.DataContext = value; }
        public ScheduleCardSelectable()
        {
            InitializeComponent();
        }
        private void Button_Close_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            ((DialogHost)button.DataContext).IsOpen = false;
        }
        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox.Text);
        }
    }
}
