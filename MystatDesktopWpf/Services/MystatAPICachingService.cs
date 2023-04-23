using MystatAPI;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace MystatDesktopWpf.Services
{
    internal static class MystatAPICachingService
	{
		private static readonly MystatAPIClient api;
		private static string? userCachePath;
		private static string rootCachePath;


		private static string UserCachePath
		{
			get
			{
				if (userCachePath == null) throw new DirectoryNotFoundException("No user directory speficied");
				return userCachePath;
			}
		}
		public static string Login
		{
			set
			{
				string encodedLogin = Convert.ToBase64String(Encoding.UTF8.GetBytes(value));
				
				userCachePath = Directory.CreateDirectory(
					Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\cache\users\" + encodedLogin)).FullName;
			}
		}
		static MystatAPICachingService()
		{
			api = MystatAPISingleton.Client;
			try
			{
				Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat"));
				Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\cache"));
				rootCachePath = Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\cache\users")).FullName;
			}
			catch (Exception exception)
			{
				MessageBox.Show(exception.Message);
				throw;
			}
		}

		public static async Task<ProfileInfo> GetAndUpdateCachedProfileInfo()
		{
			string filePath = $"{UserCachePath}\\profileInfo.json";
			ProfileInfo? profileInfo = null;
			if (File.Exists(filePath))
			{
				string jsonData = await File.ReadAllTextAsync(filePath);
				profileInfo = JsonSerializer.Deserialize<ProfileInfo>(jsonData);
			}
			profileInfo ??= await api.GetProfileInfo();
			CreateUserCacheDir();
			UpdateCachedProfileInfo(filePath, profileInfo);
			return profileInfo;
		}

		public static async Task<MystatAPI.Entity.Activity[]?> GetCachedActivities()
		{
			string filePath = $"{UserCachePath}\\activities.json";
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
				CreateUserCacheDir();
				await File.WriteAllTextAsync($"{UserCachePath}\\activities.json", JsonSerializer.Serialize(activities));
			}
			catch (Exception)
			{
				// TODO LOG
			}
		}

		public async static Task<bool> ClearCacheAsync()
		{
			return await Task.Run(() =>
			{
				try
				{
					foreach (var directory in Directory.GetDirectories(rootCachePath))
					{
						Directory.Delete(directory, true);
					}
					return true;

				}
				catch (Exception)
				{
					return false;
				}
			});
		}

		public async static Task<long> GetCacheSize()
		{
			return await Task.Run(() =>
			{
				long sizeInBytes = 0;

				try
				{

                    string[] fileNames = Directory.GetFiles(rootCachePath, "*.*", SearchOption.AllDirectories);

                    foreach (string name in fileNames)
                    {
                        FileInfo info = new FileInfo(name);
                        sizeInBytes += info.Length;
                    }

                    return sizeInBytes;
                }
				catch (Exception) { }

				return sizeInBytes;
			});
		}

        private static void CreateUserCacheDir()
		{
			Directory.CreateDirectory(userCachePath);
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
