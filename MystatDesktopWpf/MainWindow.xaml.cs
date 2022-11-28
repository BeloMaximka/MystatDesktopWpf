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
                Visibility = Visibility.Visible,
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

            theme.SecondaryLight = new ColorPair(color.Lighten());
            theme.SecondaryMid = new ColorPair(color);
            theme.SecondaryDark = new ColorPair(color.Darken());

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

        public void BringToForeground()
        {
            if (WindowState == WindowState.Minimized || Visibility == Visibility.Hidden)
            {
                Show();
                WindowState = WindowState.Normal;
            }

            Activate();
            Topmost = true;
            Topmost = false;
            Focus();
        }

        private void WindowClosed(object sender, EventArgs e)
        {
            trayIcon.Dispose();
            SettingsService.Save();
        }

        private void OnStateChanged(object? sender, EventArgs e)
        {
            var trayBehavior = SettingsService.Settings.Tray.TrayBehavior;

            switch (WindowState)
            {
                case WindowState.Minimized:
                    if(trayBehavior != TrayBehavior.NeverMove && trayBehavior != TrayBehavior.OnlyOnClose)
                    {
                        Hide();
                    }
                    break;
                default:
                    break;
            }
        }

        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var trayBehavior = SettingsService.Settings.Tray.TrayBehavior;

            if (trayBehavior == TrayBehavior.NeverMove) return;
            if (trayBehavior == TrayBehavior.OnlyOnClose)
            {
                Hide();
            }

            WindowState = WindowState.Minimized;
            e.Cancel = !realAppClose;
        }

        #region Tray
        private void UpdateTrayText(object? sender, EventArgs e)
        {
            // TODO: Переделать метод, чтобы избавиться от непонятных индексов
            var menu = (ContextMenu)FindResource("trayIconContextMenu");
            SetLocalizedHeader(menu.Items[0], "m_ShowFromTray");
            SetLocalizedHeader(menu.Items[2], "m_TrayScheduleToday");
            SetLocalizedHeader(menu.Items[3], "m_TrayScheduleTomorrow");
            SetLocalizedHeader(menu.Items[5], "m_Exit");
        }

        private void SetLocalizedHeader(object item, string key)
        {
            var menuItem = item as MenuItem;

            if (menuItem is null) return;

            menuItem.Header = FindResource(key);
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

        private void MenuItemScheduleToday(object sender, RoutedEventArgs e)
        {
            ShowScheduleCard(DateTime.Now);
        }
        
        private void MenuItemScheduleTomorrow(object sender, RoutedEventArgs e)
        {
            ShowScheduleCard(DateTime.Now.AddDays(1));
        }

        async Task ShowScheduleCard(DateTime date)
        {
            var schedule = await MystatAPISingleton.mystatAPIClient.GetScheduleByDate(date);
            popup.Child = ScheduleControlCreator.CreateScheduleCard(schedule.ToList());
            popup.IsOpen = true;
        }

        private void popup_MouseLeave(object sender, MouseEventArgs e)
        {
            popup.IsOpen = false;
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
