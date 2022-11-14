using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.ViewModels
{
    internal class LanguageViewModel : ViewModelBase
    {
        public LanguageViewModel()
        {
            index = Languages.IndexOf(App.Language);
        }
        public List<CultureInfo> Languages => App.Languages;

        int index;
        public int SelectedIndex
        {
            get => index;
            set
            {
                index = value;
                App.Language = App.Languages[index];
                OnPropertyChanged();
                OnPropertyChanged("SelectedLanguageName");
            }
        }

        public string SelectedLanguageName { get => App.Language.TwoLetterISOLanguageName.ToUpper(); }
    }
}