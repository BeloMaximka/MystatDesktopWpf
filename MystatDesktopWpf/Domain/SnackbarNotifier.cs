using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using MaterialDesignThemes.Wpf;
using System.Windows.Navigation;

namespace MystatDesktopWpf.Domain
{
    class SnackbarNotifier
    {
        Snackbar snackbar;
        Grid message;
        TextBlock textBlock;
        public SnackbarNotifier(Snackbar snackbar)
        {
            this.snackbar = snackbar;

            Grid grid = new();
            grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new());

            PackIcon icon = new();
            icon.Kind = PackIconKind.BellAlert;
            icon.Margin = new(0, 0, 8, 0);
            grid.Children.Add(icon);

            textBlock = new();
            grid.Children.Add(textBlock);
            Grid.SetColumn(textBlock, 1);
            message = grid;
        }
        public void RaiseNotify(string text, string? sound = null, TimeSpan? duration = null)
        {
            textBlock.Text = text;
            snackbar.MessageQueue?.Enqueue(message, "OK", (bool _) => { }, false, false, false, duration);
            if (sound != null)
                SoundCachingPlayer.Play(sound);
        }
    }
}
