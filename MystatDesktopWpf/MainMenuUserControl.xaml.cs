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
using MystatAPI;
using MystatAPI.Entity;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for MainMenuUserControl.xaml
    /// </summary>
    public partial class MainMenuUserControl : UserControl
    {
        public MystatAPIClient Mystat { get; set; }
        public MainMenuUserControl()
        {
            InitializeComponent();
        }
        async void GetTestInfo()
        {
            var schedule = await Mystat.GetScheduleByDate(DateTime.Now);
            MessageBox.Show(schedule[0].TeacherName);
        }
        private void TabControl_GotFocus(object sender, RoutedEventArgs e)
        {
            GetTestInfo();
        }
    }
}
