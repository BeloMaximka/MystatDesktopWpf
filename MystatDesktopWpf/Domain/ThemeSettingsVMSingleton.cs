using MystatAPI;
using MystatAPI.Entity;
using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.Domain
{
    // Почему Singleton? Ибо кнопка переключения темы есть в разных местах программы
    static class ThemeSettingsVMSingleton
    {
        static public ThemeSettingsViewModel ViewModel { get; private set; }
        static ThemeSettingsVMSingleton()
        {
            ViewModel = new ThemeSettingsViewModel();
        }
    }
}