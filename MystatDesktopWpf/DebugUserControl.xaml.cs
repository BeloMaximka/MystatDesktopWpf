using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Domain;
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

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for DebugUserControl.xaml
    /// </summary>
    public partial class DebugUserControl : UserControl
    {
        SnackbarNotifier notifier;
        public DebugUserControl()
        {
            InitializeComponent();
            notifier = new(notifySnackbar);
        }

        private void Button_Snackbar_Click(object sender, RoutedEventArgs e)
        {
            notifier.RaiseNotify("Пара начнётся через 5 минут!");
        }
        private void Button_SnackbarTimer_Click(object sender, RoutedEventArgs e)
        {
            notifier.RaiseNotify("Пара начнётся через 5 минут!", "notification.wav", new TimeSpan(0, 0, 0, 10, 0));
        }
    }
}
