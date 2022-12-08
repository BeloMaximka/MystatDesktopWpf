using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Services;
using System;

namespace MystatDesktopWpf.Domain
{
    internal class ThemeSubSettings : ISettingsProperty
    {
        private string colorHex = "#FF673AB7";
        public string ColorHex
        {
            get => colorHex;
            set
            {
                colorHex = value;
                PropertyChanged();
            }
        }

        private bool isDarkTheme = true;
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                isDarkTheme = value;
                PropertyChanged();
            }
        }

        private bool isColorAdjusted = true;
        public bool IsColorAdjusted
        {
            get => isColorAdjusted;
            set
            {
                isColorAdjusted = value;
                PropertyChanged();
            }
        }

        private Contrast contrast = Contrast.Low;
        public Contrast Contrast
        {
            get => contrast;
            set
            {
                contrast = value;
                PropertyChanged();
            }
        }

        private float contrastRatio = 4.5f;
        public float ContrastRatio
        {
            get => contrastRatio;
            set
            {
                contrastRatio = value;
                PropertyChanged();
            }
        }

        private ColorSelection colors = ColorSelection.All;
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
