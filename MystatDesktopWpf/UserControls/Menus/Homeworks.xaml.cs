using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
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
using System.IO;
using System.Diagnostics;
using MystatAPI;
using System.IO.Compression;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for Homeworks.xaml
    /// </summary>
    public partial class Homeworks : UserControl, IHomeworkManager
    {
        public Homeworks()
        {
            InitializeComponent();

            uploadDialogContent.HomeworkManager = this;
            uploadDialogContent.Host = homeworkDialog;
        }

        public async void DownloadHomework(Homework homework)
        {
            HttpClient http = new();
            try
            {
                var res = await http.GetAsync(homework.FilePath);
                string fileName = res.Content.Headers.ContentDisposition?.FileName.Trim('\"');

                System.Windows.Forms.SaveFileDialog dialog = new();
                dialog.FileName = fileName;
                string extension = fileName.Remove(0, fileName.LastIndexOf('.'));
                dialog.Filter = $"(*{extension})|*{extension}";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var buffer = await res.Content.ReadAsByteArrayAsync();
                    File.WriteAllBytes(dialog.FileName, buffer);

                    string homeworkDownloaded = (string)FindResource("m_HomeworkDownloaded");
                    string openInExplorer = (string)FindResource("m_OpenInExplorer");
                    snackbar.MessageQueue?.Enqueue<string>(homeworkDownloaded, openInExplorer, OpenFileInExplorer, dialog.FileName);
                }
            }
            catch (Exception)
            {
                string homeworkDownloadError = (string)FindResource("m_HomeworkDownloadError");
                snackbar.MessageQueue?.Enqueue(homeworkDownloadError);
            }
        }
        void OpenFileInExplorer(string filePath)
        {
            try
            {
                string argument = "/select, \"" + filePath + "\"";
                Process.Start("explorer.exe", argument);
            }
            catch (Exception)
            {
                string folderOpenError = (string)FindResource("m_FolderOpenError");
                snackbar.MessageQueue?.Enqueue(folderOpenError);
            }
        }

        #region UploadHomework
        public void UploadHomework(int homeworkId)
        {
            var dialog = homeworkDialog.DialogContent as UploadHomeworkDialogContent;
            if (dialog != null)
            {
                dialog.HomeworkId = homeworkId;
                dialog.ResetContent();
                homeworkDialog.IsOpen = true;
            }
        }

        public async void UploadHomework(int homeworkId, string[]? files, string? comment, bool archive = false, string archiveName = "")
        {
            try
            {
                if (files != null && archive)
                {
                    using MemoryStream stream = new();
                    using (ZipArchive zip = new(stream, ZipArchiveMode.Create, true))
                    {
                        foreach (var path in files)
                            zip.CreateEntryFromAny(path);
                    }
                    HomeworkFile file = new(archiveName, stream.ToArray());

                    await MystatAPISingleton.mystatAPIClient.UploadHomeworkFile(homeworkId, file, comment);
                }
                else
                    await MystatAPISingleton.mystatAPIClient.UploadHomework(homeworkId, files?[0], comment);

                snackbar.MessageQueue?.Enqueue("Домашнее задание успешно загружено.");
            }
            catch (Exception)
            {
                snackbar.MessageQueue?.Enqueue("Произошла ошибка при загрузке домашнего задания.");
            }
            homeworkDialog.IsOpen = false;
        }
        #endregion
    }
}
