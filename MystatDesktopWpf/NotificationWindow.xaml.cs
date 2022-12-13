using MystatDesktopWpf.Domain;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        private readonly DispatcherTimer timer = new();
        private readonly bool sound;
        private readonly string message;
        private readonly TimeSpan duration;
        public NotificationWindow(string message, bool sound = true, TimeSpan? duration = null)
        {
            InitializeComponent();
            this.sound = sound;
            this.message = message;
            this.duration = duration ?? new TimeSpan(0, 0, 3);
        }

        private void CheckSnackbar(object? sender, EventArgs e)
        {
            timer.Stop();
            WindowInteropHelper wih = new(Application.Current.MainWindow);
            FlashWindow(wih.Handle, false);
            this.Close();
        }

        [DllImport("user32")] private static extern int FlashWindow(IntPtr hwnd, bool bInvert);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SnackbarNotifier notifier = new(snackbar);
            notifier.RaiseNotify(message, sound ? "notification.wav" : null, duration);
            timer.Interval = duration.Add(new TimeSpan(0, 0, 1));
            timer.Tick += CheckSnackbar;
            timer.Start();
            this.Left = SystemParameters.MaximizedPrimaryScreenWidth - this.ActualWidth;
            this.Top = SystemParameters.MaximizedPrimaryScreenHeight - this.ActualHeight * 2;

            WindowInteropHelper wih = new(Application.Current.MainWindow);
            FlashWindow(wih.Handle, true);
        }

        private void Snackbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
    }
}
