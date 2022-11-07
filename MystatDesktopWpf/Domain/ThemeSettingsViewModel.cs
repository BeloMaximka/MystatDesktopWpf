using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignColors;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignThemes.Wpf;

namespace MystatDesktopWpf.Domain
{
    internal class ThemeSettingsViewModel : ViewModelBase
    {
        private readonly PaletteHelper paletteHelper = new();

        bool isDarkTheme = false;
        public bool IsDarkTheme
        {
            get => isDarkTheme;
            set
            {
                isDarkTheme = value;
                OnPropertyChanged();

                ITheme theme = paletteHelper.GetTheme();
                IBaseTheme baseTheme = isDarkTheme ? new MaterialDesignDarkTheme() : (IBaseTheme)new MaterialDesignLightTheme();
                theme.SetBaseTheme(baseTheme);
                paletteHelper.SetTheme(theme);
            }
        }

        private bool isColorAdjusted;
        public bool IsColorAdjusted
        {
            get => isColorAdjusted;
            set
            {
                if (SetProperty(ref isColorAdjusted, value))
                {
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
        private float desiredContrastRatio = 4.5f;
        public float DesiredContrastRatio
        {
            get => desiredContrastRatio;
            set
            {
                if (SetProperty(ref desiredContrastRatio, value))
                {
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

        private Contrast contrastValue;
        public Contrast ContrastValue
        {
            get => contrastValue;
            set
            {
                if (SetProperty(ref contrastValue, value))
                {
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

        private ColorSelection colorSelectionValue;
        public ColorSelection ColorSelectionValue
        {
            get => colorSelectionValue;
            set
            {
                if (SetProperty(ref colorSelectionValue, value))
                {
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
