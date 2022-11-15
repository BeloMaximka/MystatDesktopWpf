using MystatDesktopWpf.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
