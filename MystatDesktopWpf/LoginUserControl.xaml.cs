using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using MystatAPI;
using MystatAPI.Entity;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Логика взаимодействия для LoginUserControl.xaml
    /// </summary>
    public partial class LoginUserControl : UserControl
    {
        public UserLoginData Login { get; set; }
        public MystatAPIClient Mystat { get; set; }
        public LoginUserControl()
        {
            InitializeComponent();
        }

        async void LoginToMystat()
        {
            MystatAuthResponse response;
            try
            {
                response = await Mystat.Login();
            }
            catch (Exception e)
            {
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                errorText.Text = "Cannot connect to server";
                errorText.Visibility = Visibility.Visible;
                MessageBox.Show(e.Message);
                return;
            }

            MystatAuthSuccess? responseSuccess = response as MystatAuthSuccess;
            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
            if (responseSuccess != null)
            {
                Transitioner.MoveNextCommand.Execute(null, null);
            }
            else
            {
                MystatAuthError error = response as MystatAuthError;
                errorText.Text = error.Message;
                errorText.Visibility = Visibility.Visible;
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool inputError = string.IsNullOrWhiteSpace((loginTextBox.Text ?? "").ToString()) || passwordTextBox.SecurePassword.Length == 0;
            if (!inputError)
            {
                Login.Username = loginTextBox.Text;
                Login.Password = passwordTextBox.Password;
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, true);
                errorText.Visibility = Visibility.Collapsed;
                LoginToMystat();
            }
            else
            {
                errorText.Text = "All fields are required";
                errorText.Visibility = Visibility.Visible;
            }
        }
    }
}
