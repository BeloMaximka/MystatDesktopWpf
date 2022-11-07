using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Services;

namespace MystatDesktopWpf.ViewModels
{
    public class ThemeSettingsViewModel : ViewModelBase
    {
        private readonly PaletteHelper paletteHelper = new();

        public bool IsDarkTheme
        {
            get => SettingsService.Settings.Theme.IsDarkTheme;
            set
            {
                if (SettingsService.Settings.Theme.IsDarkTheme != value)
                {
                    SettingsService.Settings.Theme.IsDarkTheme = value;
                    OnPropertyChanged();

                    ITheme theme = paletteHelper.GetTheme();
                    IBaseTheme baseTheme = value ? new MaterialDesignDarkTheme() : new MaterialDesignLightTheme();
                    theme.SetBaseTheme(baseTheme);
                    paletteHelper.SetTheme(theme);
                }
            }
        }

        public bool IsColorAdjusted
        {
            get => SettingsService.Settings.Theme.IsColorAdjusted;
            set
            {
                if (SettingsService.Settings.Theme.IsColorAdjusted != value)
                {
                    SettingsService.Settings.Theme.IsColorAdjusted = value;
                    OnPropertyChanged();

                    Theme theme = (Theme)paletteHelper.GetTheme();
                    if (value)
                    {
                        theme.ColorAdjustment = new ColorAdjustment
                        {
                            DesiredContrastRatio = DesiredContrastRatio,
                            Contrast = ContrastValue,
                            Colors = ColorSelectionValue
                        };
                    }
                    else theme.ColorAdjustment = null;
                    paletteHelper.SetTheme(theme);
                }
            }
        }
        public float DesiredContrastRatio
        {
            get => SettingsService.Settings.Theme.ContrastRatio;
            set
            {
                if (SettingsService.Settings.Theme.ContrastRatio != value)
                {
                    SettingsService.Settings.Theme.ContrastRatio = value;
                    OnPropertyChanged();

                    Theme theme = (Theme)paletteHelper.GetTheme();

                    if (theme.ColorAdjustment != null)
                    {
                        theme.ColorAdjustment.DesiredContrastRatio = value;
                        paletteHelper.SetTheme(theme);
                    }
                }
            }
        }

        public IEnumerable<Contrast> ContrastValues => Enum.GetValues(typeof(Contrast)).Cast<Contrast>();

        public Contrast ContrastValue
        {
            get => SettingsService.Settings.Theme.Contrast;
            set
            {
                if (SettingsService.Settings.Theme.Contrast != value)
                {
                    SettingsService.Settings.Theme.Contrast = value;
                    OnPropertyChanged();

                    Theme theme = (Theme)paletteHelper.GetTheme();

                    if (theme.ColorAdjustment != null)
                    {
                        theme.ColorAdjustment.Contrast = value;
                        paletteHelper.SetTheme(theme);
                    }
                }
            }
        }

        public IEnumerable<ColorSelection> ColorSelectionValues => Enum.GetValues(typeof(ColorSelection)).Cast<ColorSelection>();

        public ColorSelection ColorSelectionValue
        {
            get => SettingsService.Settings.Theme.Colors;
            set
            {
                if (SettingsService.Settings.Theme.Colors != value)
                {
                    SettingsService.Settings.Theme.Colors = value;
                    OnPropertyChanged();

                    Theme theme = (Theme)paletteHelper.GetTheme();

                    if (theme.ColorAdjustment != null)
                    {
                        theme.ColorAdjustment.Colors = value;
                        paletteHelper.SetTheme(theme);
                    }
                }
            }
        }
    }
}
