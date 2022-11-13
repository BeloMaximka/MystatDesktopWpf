using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

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
            App.LanguageChanged += App_LanguageChanged;

            Languages.Clear();
            Languages.Add(new CultureInfo("en-US")); //Нейтральная культура для этого проекта
            Languages.Add(new CultureInfo("ru-RU"));
            Languages.Add(new CultureInfo("uk-UA"));

            Language = new CultureInfo(SettingsService.Settings.Language);
        }

        //Евент для оповещения всех окон приложения
        public static event EventHandler LanguageChanged;

        public static CultureInfo Language
        {
            get
            {
                return System.Threading.Thread.CurrentThread.CurrentUICulture;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("value");
                if (value == System.Threading.Thread.CurrentThread.CurrentUICulture) return;

                //1. Меняем язык приложения:
                System.Threading.Thread.CurrentThread.CurrentUICulture = value;
                MystatAPISingleton.mystatAPIClient.SetLanguage(value.TwoLetterISOLanguageName);

                //2. Создаём ResourceDictionary для новой культуры
                ResourceDictionary dict = new ResourceDictionary();
                dict.Source = new Uri(String.Format("Languages/lang.{0}.xaml", value.Name), UriKind.Relative);

                //3. Находим старую ResourceDictionary и удаляем его и добавляем новую ResourceDictionary
                ResourceDictionary oldDict = Application.Current.Resources.MergedDictionaries
                                             .First(d => d.Source != null && d.Source.OriginalString.StartsWith("Languages/lang."));
                if (oldDict != null)
                {
                    int ind = Application.Current.Resources.MergedDictionaries.IndexOf(oldDict);
                    Application.Current.Resources.MergedDictionaries.Remove(oldDict);
                    Application.Current.Resources.MergedDictionaries.Insert(ind, dict);
                }
                else
                {
                    Application.Current.Resources.MergedDictionaries.Add(dict);
                }

                //4. Вызываем евент для оповещения всех окон.
                LanguageChanged(Application.Current, new EventArgs());
            }
        }

        private void App_LanguageChanged(Object sender, EventArgs e)
        {
            SettingsService.SetPropertyValue("Language", Language.Name);
        }

        const string UniqueEventName = "4B41F251-D34D-419C-ACCC-4144EE501BD1";
        const string UniqueMutexName = "C8C527A3-7439-47C3-9403-DF9539E62D8B";
        EventWaitHandle eventWaitHandle;
        Mutex mutex;

        protected override void OnStartup(StartupEventArgs e)
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
    }
}
