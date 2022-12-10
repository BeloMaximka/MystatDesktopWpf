using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.Domain
{
    // Почему Singleton? Ибо кнопка переключения темы есть в разных местах программы
    internal static class ThemeSettingsVMSingleton
    {
        static public ThemeSettingsViewModel ViewModel { get; private set; }
        static ThemeSettingsVMSingleton()
        {
            ViewModel = new ThemeSettingsViewModel();
        }
    }
}