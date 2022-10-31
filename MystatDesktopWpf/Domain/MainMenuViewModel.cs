using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MystatDesktopWpf.UserControls;

namespace MystatDesktopWpf.Domain
{
    internal class MainMenuViewModel : ViewModelBase
    {

        public ObservableCollection<MainMenuItem> MenuItems { get; } = new();
        int selectedIndex;
        MainMenuItem selectedItem;
        public MainMenuViewModel()
        {
            MenuItems.Add(new MainMenuItem("Главная", typeof(Placeholder), PackIconKind.Home, PackIconKind.Home));
            MenuItems.Add(new MainMenuItem("Расписание", typeof(Schedule), PackIconKind.CalendarMonth, PackIconKind.CalendarMonth));
            MenuItems.Add(new MainMenuItem("Настройки", typeof(Placeholder), PackIconKind.Cog, PackIconKind.Cog));
            MenuItems.Add(new MainMenuItem("Debug", typeof(Debug), PackIconKind.Bug, PackIconKind.Bug));
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
