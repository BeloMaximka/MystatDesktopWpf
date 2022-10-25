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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for NotificationWindow.xaml
    /// </summary>
    public partial class NotificationWindow : Window
    {
        DispatcherTimer timer = new();
        bool sound;
        string message;
        TimeSpan duration;
        public NotificationWindow(string message, bool sound = true, TimeSpan? duration = null)
        {
            InitializeComponent();
            this.sound = sound;
            this.message = message;
            this.duration = duration ?? new TimeSpan(0, 0, 3);
        }
        void CheckSnackbar(object? sender, EventArgs e)
        {
            timer.Stop();
            WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
            FlashWindow(wih.Handle, false);
            this.Close();
        }

        [DllImport("user32")] public static extern int FlashWindow(IntPtr hwnd, bool bInvert);
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SnackbarNotifier notifier = new(snackbar);
            notifier.RaiseNotify(message, sound ? "notification.wav" : null, duration);
            timer.Interval = duration.Add(new TimeSpan(0, 0, 1));
            timer.Tick += CheckSnackbar;
            timer.Start();
            this.Left = SystemParameters.MaximizedPrimaryScreenWidth - this.ActualWidth;
            this.Top = SystemParameters.MaximizedPrimaryScreenHeight - this.ActualHeight * 2;

            WindowInteropHelper wih = new WindowInteropHelper(Application.Current.MainWindow);
            FlashWindow(wih.Handle, true);
        }

        private void snackbar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }
    }
}
