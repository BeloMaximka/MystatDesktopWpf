using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal class ThemeSubSettings : ISettingsProperty
    {
        string colorHex = "#FF673AB7";
        public string ColorHex
        {
            get => colorHex;
            set
            {
                colorHex = value;
                PropertyChanged();
            }
        }

        bool isDarkTheme = true;
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                isDarkTheme = value;
                PropertyChanged();
            }
        }

        bool isColorAdjusted = true;
        public bool IsColorAdjusted
        {
            get => isColorAdjusted;
            set
            {
                isColorAdjusted = value;
                PropertyChanged();
            }
        }
        public event Action? OnPropertyChanged;

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
