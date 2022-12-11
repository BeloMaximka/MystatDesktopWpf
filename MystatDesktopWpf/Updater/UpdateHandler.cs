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
        private static readonly HttpClient httpClient = new();
        private static string tempUpdateDir;

        public static async Task<UpdateCheckResult> CheckForUpdates()
        {
            var result = await httpClient.GetAsync(@"https://github.com/BeloMaximka/MystatDesktopWpf/releases/download/latest/version");
            string remoteVersion = await result.Content.ReadAsStringAsync();

            try
            {
                string localVersion = File.ReadAllText("./version");
                return localVersion == remoteVersion ? UpdateCheckResult.NoUpdates : UpdateCheckResult.UpdateReady;
            }
            catch (IOException)
            {
                return UpdateCheckResult.NoUpdates;
            }
        }

        public static async Task DownloadUpdate()
        {
            var result = await httpClient.GetAsync(@"https://github.com/BeloMaximka/MystatDesktopWpf/releases/download/latest/MystatDesktop.zip");
            var buffer = await result.Content.ReadAsByteArrayAsync();

            tempUpdateDir = $"{Path.GetTempPath()}MystatDesktop{Path.GetRandomFileName()}";
            Directory.CreateDirectory(tempUpdateDir);

            using MemoryStream stream = new(buffer);
            using ZipArchive archive = new(stream);
            archive.ExtractToDirectory(tempUpdateDir);
        }

        public static void RequestUpdate()
        {
            string executable = Directory.GetFiles(tempUpdateDir, "*.exe")[0];
            ProcessStartInfo info = new()
            {
                FileName = executable,
                WorkingDirectory = tempUpdateDir,
                Arguments = $"-wait {Environment.ProcessId} -update \"{Directory.GetCurrentDirectory()}\""
            };
            Process.Start(info);
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
