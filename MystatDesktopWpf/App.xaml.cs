using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.Updater;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace MystatDesktopWpf
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static List<CultureInfo> Languages { get; private set; } = new List<CultureInfo>();

        public App()
        {
            InitializeComponent();
            LanguageChanged += App_LanguageChanged;

            Languages.Clear();
            Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            Languages.Add(new CultureInfo("ru-RU"));
            Languages.Add(new CultureInfo("uk-UA"));

            Language = new CultureInfo(SettingsService.Settings.Language);
        }

        //Евент для оповещения всех окон приложения
        public static event EventHandler LanguageChanged;
        public static event EventHandler GroupChanged;

        public static CultureInfo Language
        {
            get
            {
                return Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException(nameof(value));
                if (value == Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                Thread.CurrentThread.CurrentUICulture = value;
                MystatAPISingleton.Client.SetLanguage(value.TwoLetterISOLanguageName);

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new()
                {
                    Source = new Uri(string.Format("Languages/lang.{0}.xaml", value.Name), UriKind.Relative)
                };

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = Current.Resources.MergedDictionaries
                                             .First(d => d.Source != null && d.Source.OriginalString.StartsWith("Languages/lang."));
                if (oldDict != null)
                {
                    int ind = Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Current.Resources.MergedDictionaries.Remove(oldDict);
                    Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged(Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(object? sender, EventArgs e)
        {
            SettingsService.SetPropertyValue("Language", Language.Name);
        }

        public static void NotifyGroupChanged()
        {
            GroupChanged.Invoke(null, null!);
        }

        private const string UniqueEventName = "4B41F251-D34D-419C-ACCC-4144EE501BD1";
        private const string UniqueMutexName = "C8C527A3-7439-47C3-9403-DF9539E62D8B";
        private EventWaitHandle eventWaitHandle;
        private Mutex? mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            int index = Array.IndexOf(e.Args, "-wait");
            if (index != -1)
            {
                int id = int.Parse(e.Args[index + 1]);
                try
                {
                    Process process = Process.GetProcessById(id);
                    process.WaitForExit();
                }
                catch (ArgumentException) { }

                index = Array.IndexOf(e.Args, "-update");
                if (index != -1)
                {
                    try
                    {
                        UpdateHandler.Update(e.Args[index + 1]);
                    }
                    catch (IOException) { }

                    Shutdown();
                }
                index = Array.IndexOf(e.Args, "-clear");
                if (index != -1)
                {
                    Directory.Delete(e.Args[index + 1], true);
                    CheckSingleton();
                    MainWindow = new MainWindow();
                    MainWindow.Show();
                    return;
                }

            }
            CheckSingleton();

            MainWindow = e.Args.Contains("-noupdate") ? MainWindow = new MainWindow() : new UpdateWindow();
            MainWindow.Show();
        }

        private void CheckSingleton()
        {
            mutex = new Mutex(true, UniqueMutexName, out bool isOwned);
            eventWaitHandle = new EventWaitHandle(false, EventResetMode.AutoReset, UniqueEventName);

            // So, R# would not give a warning that this variable is not used.
            GC.KeepAlive(mutex);

            if (isOwned)
            {
                var thread = new Thread(() =>
                {
                    while (eventWaitHandle.WaitOne())
                    {
                        Current.Dispatcher.BeginInvoke(() => ((MainWindow)Current.MainWindow).BringToForeground());
                    }
                });

                // Чтобы поток на заблокировал закрытие приложение
                thread.IsBackground = true;

                thread.Start();
                return;
            }

            eventWaitHandle.Set();
            Shutdown();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            mutex?.Close();
        }
    }
}
