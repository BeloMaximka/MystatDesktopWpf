using IWshRuntimeLibrary;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace MystatDesktopWpf.UserControls.SettingsSections
{
	/// <summary>
	/// Interaction logic for WindowsSettings.xaml
	/// </summary>
	public partial class WindowsSettings : UserControl
	{
		public WindowsSettings()
		{
			InitializeComponent();
			StartupToggle.IsChecked = System.IO.File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Mystat Desktop.lnk");
			StartupToggle.Checked += ToggleButton_Checked;
		}

		// Create shortcut to startup folder
		private void ToggleButton_Checked(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton toggleButton)
			{
				try
				{
					string appPath = Process.GetCurrentProcess().MainModule.FileName;
					string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Mystat Desktop.lnk";

					WshShell shell = new WshShell();
					IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);

					shortcut.TargetPath = appPath;
					shortcut.WorkingDirectory = System.IO.Path.GetDirectoryName(appPath);
					shortcut.Save();
				}
				catch (Exception)
				{
					toggleButton.IsChecked = false;
				}
			}
		}

		private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
		{
			if (sender is ToggleButton toggleButton)
			{
				try
				{
					System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Startup) + @"\Mystat Desktop.lnk");
				}
				catch (Exception)
				{
					toggleButton.IsChecked = true;
				}
			}
		}
	}
}
