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
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors;
using MystatDesktopWpf.Converters;
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

            // Предварительная настройка приложения находится здесь
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            login.ParentTransitioner = transitioner;

            SoundCachingPlayer.Volume = SettingsService.Settings.ScheduleNotification.Volume;
            InitTheme(); // Загрузка темы происходит здесь, ибо в App библиотека MaterialDesign не успевает подгрузиться
            
             // notification icon initialization
            trayIcon = new TaskbarIcon()
            {
                IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/favicon.ico")),
                Visibility = Visibility.Hidden,
                ContextMenu = FindResource("trayIconContextMenu") as ContextMenu,
            };
            trayIcon.MouseDown += (_, _) => Show();
            trayIcon.DoubleClickCommand = new TrayDoubleClickCommand(() => MenuItemClickShow(null, null));

            // on minimize
            StateChanged += OnStateChanged;

            // Текст с трея через DynamicResource не обновляется, поэтому обновляем так
            App.LanguageChanged += UpdateTrayText;
        }

        void InitTheme() // Загрузка темы с настроек
        {
            PaletteHelper helper = new();
            Theme theme = (Theme)helper.GetTheme();
            ThemeSubSettings settings = SettingsService.Settings.Theme;

            Color color = ColorToHexConverter.ConvertBack(settings.ColorHex);
            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            if(settings.IsColorAdjusted)
            {
                ColorAdjustment adjustment = new();
                adjustment.Contrast = settings.Contrast;
                adjustment.DesiredContrastRatio = settings.ContrastRatio;
                adjustment.Colors = settings.Colors;
                theme.ColorAdjustment = adjustment;
            }

            IBaseTheme baseTheme = settings.IsDarkTheme ? new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);

            helper.SetTheme(theme);
        }

        private void WindowClosed(object sender, EventArgs e)
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

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            WindowState = WindowState.Minimized;
            e.Cancel = !realAppClose;
        }

        #region Tray
        private void UpdateTrayText(object? sender, EventArgs e)
        {
            var menu = (ContextMenu)FindResource("trayIconContextMenu");
            ((MenuItem)menu.Items[0]).Header = (string)FindResource("m_ShowFromTray");
            ((MenuItem)menu.Items[2]).Header = (string)FindResource("m_Exit");
        }

        private void MenuItemClickExit(object sender, RoutedEventArgs e)
        {
            realAppClose = true;
            Close();
        }

        private void MenuItemClickShow(object sender, RoutedEventArgs e)
        {
            Show();
            WindowState = WindowState.Normal;

            Topmost = true;
            Topmost = false;

            Focus();
        }
        #endregion
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
