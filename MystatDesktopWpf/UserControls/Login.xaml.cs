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
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LoginUserControl.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Transitioner? ParentTransitioner { get; set; }
        public Login()
        {
            InitializeComponent();
            if (SettingsService.Settings.LoginData != null)
            {
                loginTextBox.Text = SettingsService.Settings.LoginData.Username;
                passwordTextBox.Password = SettingsService.Settings.LoginData.Password;
                LoginToMystat(loginTextBox.Text, passwordTextBox.Password);
            }
        }

        async void LoginToMystat(string username, string password)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, true);
            errorText.Visibility = Visibility.Collapsed;

            MystatAuthResponse response;
            UserLoginData loginData = new UserLoginData(username, password);
            MystatAPISingleton.mystatAPIClient.SetLoginData(loginData);
            try
            {
                response = await MystatAPISingleton.mystatAPIClient.Login();
            }
            catch (Exception e)
            {
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                errorText.Text = "Не удалось подключиться к серверу";
                errorText.Visibility = Visibility.Visible;
                return;
            }

            MystatAuthSuccess? responseSuccess = response as MystatAuthSuccess;
            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
            if (responseSuccess != null)
            {
                SettingsService.SetLoginData(loginData);
                await ScheduleNotificationService.Configure();
                Transitioner.MoveNextCommand.Execute(null, ParentTransitioner);
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
                LoginToMystat(loginTextBox.Text, passwordTextBox.Password);
            }
            else
            {
                errorText.Text = "Все поля обязательны";
                errorText.Visibility = Visibility.Visible;
            }
        }

        private void UserControl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(loginButton, null);
            }
        }
    }
}
