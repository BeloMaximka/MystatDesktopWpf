﻿using MystatDesktopWpf.Services;
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

namespace MystatDesktopWpf.UserControls.SettingsSections
{
	/// <summary>
	/// Interaction logic for CacheSettings.xaml
	/// </summary>
	public partial class CacheSettings : UserControl
	{
		public CacheSettings()
		{
			InitializeComponent();
			UpdateCacheSize();
		}

		private async void ClearCacheButton_Click(object sender, RoutedEventArgs e)
		{
			ClearCacheButton.IsEnabled = false;
			ClearCacheButton.Content = "Clearing...";
			if (await MystatAPICachingService.ClearCacheAsync())
			{
				ClearCacheButton.Content = "Cleared!";
				UpdateCacheSize();
			}
			else
			{
				ClearCacheButton.Content = "Error!";
			}
			await Task.Delay(2000);
			ClearCacheButton.IsEnabled = true;
			ClearCacheButton.Content = "Clear cache";
		}

		public async void UpdateCacheSize()
		{
			CacheSizeTextBlock.Text = "Calculating...";
			CacheSizeTextBlock.Text = Math.Round(await MystatAPICachingService.GetCacheSize() / 1024.0, 2) + " KB";
		}
	}
}
