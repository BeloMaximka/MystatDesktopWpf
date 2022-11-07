﻿using MystatAPI.Entity;
using MystatAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.Domain;
using MaterialDesignColors.ColorManipulation;
using MaterialDesignColors;
using MystatDesktopWpf.Converters;

namespace MystatDesktopWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Application.Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
            login.ParentTransitioner = transitioner;

            SoundCachingPlayer.Volume = SettingsService.Settings.ScheduleNotification.Volume;

            PaletteHelper helper = new();
            Theme theme = (Theme)helper.GetTheme();

            Color color = ColorToHexConverter.ConvertBack(SettingsService.Settings.Theme.ColorHex);
            theme.PrimaryLight = new ColorPair(color.Lighten());
            theme.PrimaryMid = new ColorPair(color);
            theme.PrimaryDark = new ColorPair(color.Darken());

            ColorAdjustment adjustment = new();
            adjustment.Contrast = Contrast.Low;
            adjustment.DesiredContrastRatio = 3.0f;
            adjustment.Colors = ColorSelection.All;

            helper.SetTheme(theme);
        }

        private void window_Closed(object sender, EventArgs e)
        {
            SettingsService.Save();
        }
    }
}
