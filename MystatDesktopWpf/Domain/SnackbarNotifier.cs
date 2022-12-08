using MaterialDesignThemes.Wpf;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.Domain
{
    internal class SnackbarNotifier
    {
        private readonly Snackbar snackbar;
        private readonly Grid message;
        private readonly TextBlock textBlock;
        public SnackbarNotifier(Snackbar snackbar)
        {
            this.snackbar = snackbar;
            snackbar.MouseDown += Snackbar_MouseDown;

            Grid grid = new();
            grid.ColumnDefinitions.Add(new() { Width = GridLength.Auto });
            grid.ColumnDefinitions.Add(new());

            PackIcon icon = new()
            {
                Kind = PackIconKind.BellAlert,
                Margin = new(0, 0, 8, 0)
            };
            grid.Children.Add(icon);

            textBlock = new();
            grid.Children.Add(textBlock);
            Grid.SetColumn(textBlock, 1);
            message = grid;
        }

        private void Snackbar_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ((Snackbar)sender).IsActive = false;
        }

        public void RaiseNotify(string text, string? sound = null, TimeSpan? duration = null)
        {
            textBlock.Text = text;
            snackbar.MessageQueue?.Enqueue(message, null, null, false, false, false, duration);
            if (sound != null)
                SoundCachingPlayer.Play(sound);
        }
    }
}
