using MystatDesktopWpf.Services;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Updater
{
    internal static class UpdateHandler
    {
        public static event Action? UpdateReady;
        public static event Action? UpdateStarted;
        public static event Action? UpdateCancelled;

        private static readonly HttpClient httpClient = new();
        private static string? tempUpdateDir;


        public static void ScheduleUpdateCheck()
        {
            TaskService.CancelTask("update-check");
            TaskService.ScheduleTask("update-check", DateTime.Now.AddDays(1), new TimeOnly(2, 0), CheckForUpdatesScheduled);
        }

        private static async void CheckForUpdatesScheduled()
        {
            if (await CheckForUpdates() == UpdateCheckResult.NoUpdates)
                ScheduleUpdateCheck();
        }

        public static async Task<UpdateCheckResult> CheckForUpdates()
        {
            try
            {
                var result = await httpClient.GetAsync(@"https://github.com/BeloMaximka/MystatDesktopWpf/releases/download/latest/version");
                string remoteVersion = await result.Content.ReadAsStringAsync();

                string localVersion = File.ReadAllText("./version");
                if (localVersion != remoteVersion)
                {
                    UpdateReady?.Invoke();
                    return UpdateCheckResult.UpdateReady;
                }
            }
            catch (IOException) { }
            catch (HttpRequestException) { }
            return UpdateCheckResult.NoUpdates;
        }

        private static async Task DownloadUpdate()
        {
            if (tempUpdateDir != null) return;

            var result = await httpClient.GetAsync(@"https://github.com/BeloMaximka/MystatDesktopWpf/releases/download/latest/MystatDesktop.zip");
            var buffer = await result.Content.ReadAsByteArrayAsync();

            tempUpdateDir = $"{Path.GetTempPath()}MystatDesktop{Path.GetRandomFileName()}";
            Directory.CreateDirectory(tempUpdateDir);

            using MemoryStream stream = new(buffer);
            using ZipArchive archive = new(stream);
            archive.ExtractToDirectory(tempUpdateDir);
        }

        public async static Task RequestUpdate()
        {
            UpdateStarted?.Invoke();
            try
            {
                await DownloadUpdate();
                if (tempUpdateDir == null) return;
                string executable = Directory.GetFiles(tempUpdateDir, "*.exe")[0];
                ProcessStartInfo info = new()
                {
                    FileName = executable,
                    WorkingDirectory = tempUpdateDir,
                    Arguments = $"-wait {Environment.ProcessId} -update \"{Directory.GetCurrentDirectory()}\""
                };
                Process.Start(info);
                App.Current.Shutdown();
            }
            catch (Exception)
            {
                UpdateCancelled?.Invoke();
            }
        }

        public static void Update(string updateDir)
        {
            string tempDir = updateDir + Path.GetRandomFileName();
            Directory.Move(updateDir, tempDir);
            Directory.Delete(tempDir, true);

            CopyDirectory(Directory.GetCurrentDirectory(), updateDir, true);

            string executable = Directory.GetFiles(updateDir, "*.exe")[0];
            ProcessStartInfo info = new()
            {
                FileName = executable,
                WorkingDirectory = updateDir,
                Arguments = $"-wait {Environment.ProcessId} -clear \"{Directory.GetCurrentDirectory()}\""
            };
            Process.Start(info);
        }

        private static void CopyDirectory(string source, string destination, bool recursive)
        {
            var dir = new DirectoryInfo(source);

            if (!dir.Exists)
                throw new DirectoryNotFoundException($"Source directory not found: {dir.FullName}");

            DirectoryInfo[] dirs = dir.GetDirectories();
            Directory.CreateDirectory(destination);

            foreach (FileInfo file in dir.GetFiles())
            {
                string targetFilePath = Path.Combine(destination, file.Name);
                file.CopyTo(targetFilePath);
            }

            if (recursive)
            {
                foreach (DirectoryInfo subDir in dirs)
                {
                    string newDestinationDir = Path.Combine(destination, subDir.Name);
                    CopyDirectory(subDir.FullName, newDestinationDir, true);
                }
            }
        }
    }

    public enum UpdateCheckResult
    {
        UpdateReady,
        NoUpdates
    }
}
