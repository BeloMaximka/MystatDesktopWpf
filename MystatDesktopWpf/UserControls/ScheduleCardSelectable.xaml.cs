using MaterialDesignThemes.Wpf;
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
            Button button = sender as Button;
            ((DialogHost)button.DataContext).IsOpen = false;
        }
        private void Button_Copy_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Clipboard.SetText(textBox.Text);
        }
    }
}
