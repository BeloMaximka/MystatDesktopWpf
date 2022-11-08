using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
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
    }
}
