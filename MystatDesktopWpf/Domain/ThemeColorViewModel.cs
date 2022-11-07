using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace MystatDesktopWpf.Domain
{
    internal class ThemeColorViewModel : ViewModelBase
    {
        public ThemeColorViewModel()
        {
            ITheme theme = paletteHelper.GetTheme();
            SelectedColor = theme.PrimaryMid.Color;
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

                ITheme theme = paletteHelper.GetTheme();

                theme.PrimaryLight = new ColorPair(selectedColor.Lighten());
                theme.PrimaryMid = new ColorPair(selectedColor);
                theme.PrimaryDark = new ColorPair(selectedColor.Darken());

                paletteHelper.SetTheme(theme);
            }
        }
    }
}
