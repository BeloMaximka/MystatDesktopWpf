using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Логика взаимодействия для LoginUserControl.xaml
    /// </summary>
    public partial class Login : UserControl
    {
        public Transitioner? ParentTransitioner { get; set; }
        // Добавил event, чтобы пункты главного меню загружались лишь после успешнго логина (и не долбились в апишку)
        public event Action SuccessfulLogin;
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

        private async void LoginToMystat(string username, string password)
        {
            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, true);
            errorText.Visibility = Visibility.Collapsed;

            MystatAuthResponse response;
            UserLoginData loginData = new(username, password);
            MystatAPISingleton.Client.SetLoginData(loginData);
            try
            {
                response = await MystatAPISingleton.LoginAndGetProfileInfo();
            }
            catch (Exception e)
            {
                ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
                errorText.Text = (string)App.Current.FindResource("m_ConnectionFailed");
                errorText.Visibility = Visibility.Visible;
                return;
            }

            ButtonProgressAssist.SetIsIndicatorVisible(loginButton, false);
            if (response is MystatAuthSuccess responseSuccess)
            {
                SuccessfulLogin?.Invoke();

                if (dontRememberMeCheckBox.IsChecked == false)
                    SettingsService.SetLoginData(loginData);

                var schedule = SettingsService.Settings.ScheduleNotification;
                ScheduleNotificationService.OnlyFirstSchedule = schedule.OnlyFirstSchedule;
                ScheduleNotificationService.OnTimerElapsed += ShowNotification;

                if (schedule.Enabled)
                    await ScheduleNotificationService.Configure(schedule.Delay, schedule.Mode);

                Transitioner.MoveNextCommand.Execute(null, ParentTransitioner);

                loginTextBox.Text = "";
                passwordTextBox.Password = "";
            }
            else
            {
                MystatAuthError error = response as MystatAuthError;
                errorText.Text = error.Message;
                errorText.Visibility = Visibility.Visible;
            }
        }

        private void ShowNotification(DaySchedule schedule, int delay)
        {
            string message;
            if (delay > 0)
            {
                string label1 = (string)App.Current.FindResource("m_LessonStarting0");
                string label2 = (string)App.Current.FindResource("m_LessonStarting1");
                message = $"{label1} {delay} {label2}";
            }

            else
                message = (string)App.Current.FindResource("m_LessonStarted"); ;
            new NotificationWindow(message, true).Show();
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
                errorText.Text = (string)App.Current.FindResource("m_AllFieldsRequired");
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
