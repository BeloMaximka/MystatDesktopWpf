using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MystatDesktopWpf.UserControls;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.UserControls.Menus;

namespace MystatDesktopWpf.ViewModels
{
    internal class MainMenuViewModel : ViewModelBase
    {

        public ObservableCollection<MainMenuItem> MenuItems { get; } = new();
        int selectedIndex;
        MainMenuItem selectedItem;

        public MainMenuViewModel()
        {
            MenuItems.Add(new MainMenuItem("m_Schedule", typeof(Placeholder), PackIconKind.Home, PackIconKind.Home));
            MenuItems.Add(new MainMenuItem("m_Schedule", typeof(Homeworks), PackIconKind.Server, PackIconKind.Server));
            MenuItems.Add(new MainMenuItem("m_Schedule", typeof(Schedule), PackIconKind.CalendarMonth, PackIconKind.CalendarMonth));
            MenuItems.Add(new MainMenuItem("m_Settings", typeof(Settings), PackIconKind.Cog, PackIconKind.Cog));
            //MenuItems.Add(new MainMenuItem("Debug", typeof(Debug), PackIconKind.Bug, PackIconKind.Bug));
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
