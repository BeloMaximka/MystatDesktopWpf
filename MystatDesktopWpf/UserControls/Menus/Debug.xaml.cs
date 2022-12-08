using MystatDesktopWpf.Domain;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for DebugUserControl.xaml
    /// </summary>
    public partial class Debug : UserControl
    {
        private readonly SnackbarNotifier notifier;
        public Debug()
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
        private void Button_Notification_Click(object sender, RoutedEventArgs e)
        {
            new NotificationWindow("Пара начнётся через 15 минут!").Show();
        }
        private void Button_NotificationDelayed_Click(object sender, RoutedEventArgs e)
        {
            DispatcherTimer timer = new() { Interval = TimeSpan.FromSeconds(5) };
            timer.Tick += DelayedNotification;
            timer.Start();
        }

        private void DelayedNotification(object? sender, EventArgs e)
        {
            new NotificationWindow("Пара начнётся через 15 минут!", true).Show();
            ((DispatcherTimer)sender).Stop();
        }

        private void Button_EN_Click(object sender, RoutedEventArgs e)
        {
            App.Language = App.Languages[0];
        }

        private void Button_RU_Click(object sender, RoutedEventArgs e)
        {
            App.Language = App.Languages[1];
        }
    }
}
