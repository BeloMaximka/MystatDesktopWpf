using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal class MainMenuViewModel : ViewModelBase
    {

        public ObservableCollection<MainMenuItem> MenuItems { get; } = new();
        int selectedIndex;
        MainMenuItem selectedItem;
        public MainMenuViewModel()
        {
            MenuItems.Add(new MainMenuItem("Главная", typeof(PlaceholderUserControl), PackIconKind.Home, PackIconKind.Home));
            MenuItems.Add(new MainMenuItem("Расписание", typeof(PlaceholderUserControl), PackIconKind.CalendarMonth, PackIconKind.CalendarMonth));
            MenuItems.Add(new MainMenuItem("Настройки", typeof(PlaceholderUserControl), PackIconKind.Cog, PackIconKind.Cog));
            SelectedIndex = 0;
        }
        public MainMenuItem? SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public int SelectedIndex
        {
            get => selectedIndex;
            set
            {
                SetProperty(ref selectedIndex, value);
                SelectedItem = MenuItems[selectedIndex];
            }
        }
    }
}
