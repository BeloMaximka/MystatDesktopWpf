using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.Converters;

namespace MystatDesktopWpf.ViewModels
{
    internal class ThemeColorViewModel : ViewModelBase
    {
        public ThemeColorViewModel()
        {
            SelectedColor = ColorToHexConverter.ConvertBack(SettingsService.Settings.Theme.ColorHex);
        }
        private readonly PaletteHelper paletteHelper = new();

        Color selectedColor;
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                OnPropertyChanged();

                SettingsService.Settings.Theme.ColorHex = ColorToHexConverter.Convert(value);

                ITheme theme = paletteHelper.GetTheme();

                theme.PrimaryLight = new ColorPair(selectedColor.Lighten());
                theme.PrimaryMid = new ColorPair(selectedColor);
                theme.PrimaryDark = new ColorPair(selectedColor.Darken());

                paletteHelper.SetTheme(theme);
            }
        }
    }
}
