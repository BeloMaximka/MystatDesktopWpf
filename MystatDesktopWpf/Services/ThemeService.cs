using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Converters;
using System.Windows.Media;

namespace MystatDesktopWpf.Services
{
    internal static class ThemeService
    {
        static public void InitTheme() // Загрузка темы с настроек
        {
            PaletteHelper helper = new();
            Theme theme = (Theme)helper.GetTheme();
            ThemeSubSettings settings = SettingsService.Settings.Theme;

            Color color = ColorToHexConverter.ConvertBack(settings.ColorHex);

            theme.SecondaryLight = new ColorPair(color.Lighten());
            theme.SecondaryMid = new ColorPair(color);
            theme.SecondaryDark = new ColorPair(color.Darken());

            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            if (settings.IsColorAdjusted)
            {
                ColorAdjustment adjustment = new()
                {
                    Contrast = settings.Contrast,
                    DesiredContrastRatio = settings.ContrastRatio,
                    Colors = settings.Colors
                };
                theme.ColorAdjustment = adjustment;
            }

            IBaseTheme baseTheme = settings.IsDarkTheme ? new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();
            theme.SetBaseTheme(baseTheme);

            helper.SetTheme(theme);
        }
    }
}
