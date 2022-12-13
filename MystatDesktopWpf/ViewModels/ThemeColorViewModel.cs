using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Converters;
using MystatDesktopWpf.Services;
using System.Windows.Media;

namespace MystatDesktopWpf.ViewModels
{
    internal class ThemeColorViewModel : ViewModelBase
    {
        public ThemeColorViewModel()
        {
            SelectedColor = ColorToHexConverter.ConvertBack(SettingsService.Settings.Theme.ColorHex);
        }
        private readonly PaletteHelper paletteHelper = new();
        private Color selectedColor;
        public Color SelectedColor
        {
            get => selectedColor;
            set
            {
                selectedColor = value;
                OnPropertyChanged();

                SettingsService.Settings.Theme.ColorHex = ColorToHexConverter.Convert(value);

                ITheme theme = paletteHelper.GetTheme();

                theme.SecondaryLight = new ColorPair(selectedColor.Lighten());
                theme.SecondaryMid = new ColorPair(selectedColor);
                theme.SecondaryDark = new ColorPair(selectedColor.Darken());

                theme.PrimaryLight = new ColorPair(selectedColor.Lighten());
                theme.PrimaryMid = new ColorPair(selectedColor);
                theme.PrimaryDark = new ColorPair(selectedColor.Darken());

                paletteHelper.SetTheme(theme);
            }
        }
    }
}
