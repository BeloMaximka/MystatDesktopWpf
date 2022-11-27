using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    public static class ZipArchiveExtension
    {
        public static void CreateEntryFromAny(this ZipArchive archive, string path, string entryName = "")
        {
            var fileName = Path.GetFileName(path);
            if (Directory.Exists(path))
                archive.CreateEntryFromDirectory(path, Path.Combine(entryName, fileName));
            else
                archive.CreateEntryFromFile(path, Path.Combine(entryName, fileName));
        }

        public static void CreateEntryFromDirectory(this ZipArchive archive, string path, string entryName = "")
        {
            string[] files = Directory.GetFiles(path).Concat(Directory.GetDirectories(path)).ToArray();
            foreach (var file in files)
                archive.CreateEntryFromAny(file, entryName);
        }
    }
}
