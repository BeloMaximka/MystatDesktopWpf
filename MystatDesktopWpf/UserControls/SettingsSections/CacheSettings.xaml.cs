using MystatDesktopWpf.Services;
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
			ClearCacheButton.SetResourceReference(Button.ContentProperty, "m_Clearing");
			if (await MystatAPICachingService.ClearCacheAsync())
			{
				ClearCacheButton.SetResourceReference(Button.ContentProperty, "m_Cleared");
			}
			else
			{
				ClearCacheButton.SetResourceReference(Button.ContentProperty, "m_FailedToClear");
			}
			UpdateCacheSize();
			await Task.Delay(2000);
			ClearCacheButton.IsEnabled = true;
			ClearCacheButton.SetResourceReference(Button.ContentProperty, "m_ClearCache");
		}

		public async void UpdateCacheSize()
		{
			CacheSizeTextBlock.SetResourceReference(TextBlock.TextProperty, "m_Calculating");

			long cacheSizeInBytes = await MystatAPICachingService.GetCacheSize();
            CacheSizeTextBlock.Text = SizeFormatterService.Format(cacheSizeInBytes);
		}
	}
}
