using MystatAPI;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MystatDesktopWpf.Services
{
	internal static class MystatAPICachingService
	{
		private static readonly MystatAPIClient api;
		private static readonly string cacheRootPath;
		static MystatAPICachingService()
		{
			api = MystatAPISingleton.Client;
			try
			{
				Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat"));
				Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\cache"));
				cacheRootPath = Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\cache\users")).FullName;
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				throw;
			}
		}

		public static async Task<ProfileInfo> GetAndUpdateCachedProfileInfo()
		{
			string filePath = $"{cacheRootPath}\\profileInfo.json";
			ProfileInfo? profileInfo = null;
			if (File.Exists(filePath))
			{
				string jsonData = await File.ReadAllTextAsync(filePath);
				profileInfo = JsonSerializer.Deserialize<ProfileInfo>(jsonData);
			}
			profileInfo ??= await api.GetProfileInfo();
			UpdateCachedProfileInfo(filePath, profileInfo);
			return profileInfo;
		}

		public static async Task<MystatAPI.Entity.Activity[]?> GetCachedActivities()
		{
			string filePath = $"{cacheRootPath}\\activities.json";
			MystatAPI.Entity.Activity[]? activities = null;
			if (File.Exists(filePath))
			{
				string jsonData = await File.ReadAllTextAsync(filePath);
				activities = JsonSerializer.Deserialize<MystatAPI.Entity.Activity[]>(jsonData);
			}
			return activities;
		}

		public static async void UpdateCachedActivities(MystatAPI.Entity.Activity[] activities)
		{
			try
			{
				await File.WriteAllTextAsync($"{cacheRootPath}\\activities.json", JsonSerializer.Serialize(activities));
			}
			catch (Exception)
			{
				// TODO LOG
			}
		}

		private async static void UpdateCachedProfileInfo(string path, ProfileInfo? profileInfo = null)
		{
			try
			{
				profileInfo ??= await api.GetProfileInfo();
				await File.WriteAllTextAsync(path, JsonSerializer.Serialize(profileInfo));
			}
			catch (Exception)
			{
				// TODO LOG
			}
		}
	}
}
