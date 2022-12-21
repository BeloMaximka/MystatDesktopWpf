using Hardcodet.Wpf.TaskbarNotification;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.Updater;
using MystatDesktopWpf.UserControls;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly TaskbarIcon trayIcon;
        private bool realAppClose = false;

        public MainWindow()
        {
            InitializeComponent();

            // Предварительная настройка приложения находится здесь
            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            login.ParentTransitioner = transitioner;
            login.SuccessfulLogin += LoadMainMenu;

            SoundCachingPlayer.Volume = SettingsService.Settings.ScheduleNotification.Volume;

            // notification icon initialization
            trayIcon = new TaskbarIcon()
            {
                IconSource = new BitmapImage(new Uri("pack://application:,,,/Resources/favicon.ico")),
                ContextMenu = FindResource("trayIconContextMenu") as ContextMenu,
            };
            trayIcon.MouseDown += (_, _) => Show();
            trayIcon.DoubleClickCommand = new TrayDoubleClickCommand(() => MenuItemClickShow(null, null));

            // on minimize
            StateChanged += OnStateChanged;

            // Текст с трея через DynamicResource не обновляется, поэтому обновляем так
            App.LanguageChanged += UpdateTrayText;

            UpdateHandler.ScheduleUpdateCheck();
            UpdateHandler.UpdateStarted += () => SetUpdateItemStatus(true);
            UpdateHandler.UpdateCancelled += () => SetUpdateItemStatus(false);
        }

        private void LoadMainMenu()
        {
            mainMenuSlide.Content = new MainMenu();
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
                    if (trayBehavior != TrayBehavior.NeverMove && trayBehavior != TrayBehavior.OnlyOnClose)
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
            var menu = (ContextMenu)FindResource("trayIconContextMenu");
            foreach (var item in menu.Items)
            {
                if (item is MenuItem)
                {
                    MenuItem menuItem = (MenuItem)item;
                    menuItem.Header = FindResource((string)menuItem.Tag);
                }
            }
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

        private async Task ShowScheduleCard(DateTime date)
        {
            var schedule = await MystatAPISingleton.Client.GetScheduleByDate(date);
            popup.Child = ScheduleControlCreator.CreateScheduleCard(schedule.ToList());
            popup.IsOpen = true;
        }

        private void Popup_MouseLeave(object sender, MouseEventArgs e)
        {
            popup.IsOpen = false;
        }

        private async void MenuItemCheckUpdates(object sender, RoutedEventArgs e)
        {
            MenuItem item = (MenuItem)sender;
            item.IsHitTestVisible = false;
            item.Tag = "m_CheckingUpdates";
            item.Header = FindResource("m_CheckingUpdates");

            bool updateReady = await UpdateHandler.CheckForUpdates() == UpdateCheckResult.UpdateReady;
            item.Tag = updateReady ? "m_UpdateApp" : "m_CheckUpdates";
            item.Header = FindResource((string)item.Tag);
            item.IsHitTestVisible = true;

            if (updateReady)
            {
                item.Click -= MenuItemCheckUpdates;
                item.Click += MenuItemUpdate;
            }
        }

        private async void MenuItemUpdate(object sender, RoutedEventArgs e)
        {
            await UpdateHandler.RequestUpdate();
        }

        private void SetUpdateItemStatus(bool loading)
        {
            ContextMenu menu = (ContextMenu)FindResource("trayIconContextMenu");
            MenuItem item = (MenuItem)LogicalTreeHelper.FindLogicalNode(menu, "updateItem");

            item.Tag = loading ? "m_DownloadingUpdate" : "m_UpdateApp";
            item.Header = FindResource((string)item.Tag);
            item.IsHitTestVisible = true;
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
