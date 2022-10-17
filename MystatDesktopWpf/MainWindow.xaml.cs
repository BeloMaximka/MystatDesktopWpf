using MystatAPI.Entity;
using MystatAPI;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        UserLoginData login;
        MystatAPIClient mystat;
        public MainWindow()
        {
            login = new UserLoginData();
            mystat = new MystatAPIClient(login);
            InitializeComponent();
            loginUserControl.Login = login;
            loginUserControl.Mystat = mystat;
            mainMenuUserControl.Mystat = mystat;
        }
    }
}
