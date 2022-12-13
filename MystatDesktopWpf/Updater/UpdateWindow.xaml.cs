﻿using System;
using System.Windows;

namespace MystatDesktopWpf.Updater
{
    /// <summary>
    /// Interaction logic for UpdateWindow.xaml
    /// </summary>
    public partial class UpdateWindow : Window
    {
        public UpdateWindow()
        {
            InitializeComponent();
            CheckForUpdates();
        }

        private async void CheckForUpdates()
        {
            try
            {
                if (await UpdateHandler.CheckForUpdates() == UpdateCheckResult.UpdateReady)
                {
                    statusTextBox.Text = (string)App.Current.FindResource("m_DownloadingUpdate");
                    await UpdateHandler.RequestUpdate();
                }
            }
            catch (Exception) { } // TODO Log

            App.Current.MainWindow = new MainWindow();
            App.Current.MainWindow.Show();

            Close();
        }
    }
}