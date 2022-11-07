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
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.Domain;
using Hardcodet.Wpf.TaskbarNotification;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TaskbarIcon trayIcon;
        bool realAppClose = false;

        public MainWindow()
        {
            InitializeComponent();
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            login.ParentTransitioner = transitioner;

            PaletteHelper helper = new PaletteHelper();
            Theme theme = (Theme)helper.GetTheme();

            ColorAdjustment adjustment = new();
            adjustment.Contrast = Contrast.Low;
            adjustment.DesiredContrastRatio = 3.0f;
            adjustment.Colors = ColorSelection.All;

            theme.ColorAdjustment = adjustment;
            helper.SetTheme(theme);

            // notification icon initialization
            trayIcon = new TaskbarIcon()
            {
                ToolTipText = "Mystat Desktop",
                IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/favicon.ico")),
                Visibility = Visibility.Hidden,
                ContextMenu = FindResource("trayIconContextMenu") as ContextMenu
            };
            trayIcon.MouseDown += (_, _) => Show();
            trayIcon.DoubleClickCommand = new TrayDoubleClickCommand(() => MenuItem_Click_1(null, null));

            // on minimize
            StateChanged += OnStateChanged;
        }

        private void window_Closed(object sender, EventArgs e)
        {
            trayIcon.Dispose();
            SettingsService.Save();
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Minimized:
                    Hide();
                    trayIcon.Visibility = Visibility.Visible;
                    break;
                default:
                    trayIcon.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        private void window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Cancel = !realAppClose;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            realAppClose = true;
            Close();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;

            Topmost = true;
            Topmost = false;

            Focus();
        }
    }

    internal class TrayDoubleClickCommand : ICommand
    {
        public Action Callback { get; }

        public event EventHandler? CanExecuteChanged;

        public TrayDoubleClickCommand(Action callback)
        {
            Callback = callback;
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            Callback();
        }
    }
}
