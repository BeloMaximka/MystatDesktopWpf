using MaterialDesignThemes.Wpf;
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

        Contrast contrast = Contrast.Low;
        public Contrast Contrast
        {
            get => contrast;
            set
            {
                contrast = value;
                PropertyChanged();
            }
        }

        float contrastRatio = 3.0f;
        public float ContrastRatio
        {
            get => contrastRatio;
            set
            {
                contrastRatio = value;
                PropertyChanged();
            }
        }

        ColorSelection colors = ColorSelection.All;
        public ColorSelection Colors
        {
            get => colors;
            set
            {
                colors = value;
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
