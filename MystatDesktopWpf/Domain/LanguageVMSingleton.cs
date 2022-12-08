using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.Domain
{
    internal class LanguageVMSingleton
    {
        static public LanguageViewModel ViewModel { get; private set; }
        static LanguageVMSingleton()
        {
            ViewModel = new LanguageViewModel();
        }
    }
}
