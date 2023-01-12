using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.UserControls;
using MystatDesktopWpf.UserControls.Menus;
using System.Collections.ObjectModel;

namespace MystatDesktopWpf.ViewModels
{
    internal class MainMenuViewModel : ViewModelBase
    {

        public ObservableCollection<MainMenuItem> MenuItems { get; } = new();

        private int selectedIndex;
        private MainMenuItem selectedItem;

        public MainMenuViewModel()
        {
            //MenuItems.Add(new MainMenuItem("m_Schedule", typeof(Placeholder), PackIconKind.Home, PackIconKind.Home));
            MenuItems.Add(new MainMenuItem("m_Main", typeof(MainPage), PackIconKind.Poll, PackIconKind.Poll));
            MenuItems.Add(new MainMenuItem("m_Homework", typeof(Homeworks), PackIconKind.Server, PackIconKind.Server, new HomeworksViewModel()));
            MenuItems.Add(new MainMenuItem("m_Schedule", typeof(Schedule), PackIconKind.CalendarMonth, PackIconKind.CalendarMonth));
            MenuItems.Add(new MainMenuItem("m_Settings", typeof(Settings), PackIconKind.Cog, PackIconKind.Cog));
            //MenuItems.Add(new MainMenuItem("Debug", typeof(Debug), PackIconKind.Bug, PackIconKind.Bug));
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
