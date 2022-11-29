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
using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.ViewModels;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for Homeworks.xaml
    /// </summary>
    public partial class Homeworks : UserControl
    {
        HomeworksViewModel viewModel;
        public Homeworks()
        {
            InitializeComponent();

            viewModel = (HomeworksViewModel)FindResource("HomeworksViewModel");
            uploadDialog.Host = homeworkDialog;
            uploadDialog.UploadConfirm += OnUploadDialogConfirm;
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
        Button progressButton;
        Button uploadButton;
        public void OpenUploadDialog(Homework homework, ICollection<Homework> source, Button progressButton, Button uploadButton)
        {
            this.progressButton = progressButton;
            this.uploadButton = uploadButton;
            var dialog = homeworkDialog.DialogContent as UploadHomeworkDialogContent;
            if (dialog != null)
            {
                dialog.Homework = homework;
                dialog.HomeworkSource = source;
                dialog.ResetContent();
                homeworkDialog.IsOpen = true;
            }
        }

        public async void OnUploadDialogConfirm()
        {
            homeworkDialog.IsOpen = false;
            Button progress = progressButton;
            Button upload = uploadButton;

            ButtonProgressAssist.SetIsIndicatorVisible(progress, true);
            upload.IsHitTestVisible = false;
            try
            {
                if (uploadDialog.Files != null && uploadDialog.Archive)
                {
                    using MemoryStream stream = new();
                    using (ZipArchive zip = new(stream, ZipArchiveMode.Create, true))
                    {
                        foreach (var path in uploadDialog.Files)
                            zip.CreateEntryFromAny(path);
                    }
                    HomeworkFile file = new(uploadDialog.ArchiveName, stream.ToArray());

                    await MystatAPISingleton.mystatAPIClient.UploadHomeworkFile(uploadDialog.Homework.Id, file, uploadDialog.Comment);
                }
                else
                    await MystatAPISingleton.mystatAPIClient.UploadHomework(uploadDialog.Homework.Id, uploadDialog.Files?[0], uploadDialog.Comment);

                uploadDialog.HomeworkSource.Remove(uploadDialog.Homework);
                viewModel.Uploaded.Add(uploadDialog.Homework);
                snackbar.MessageQueue?.Enqueue("Домашнее задание успешно загружено.");
            }
            catch (Exception)
            {
                snackbar.MessageQueue?.Enqueue("Произошла ошибка при загрузке домашнего задания.");
            }
            ButtonProgressAssist.SetIsIndicatorVisible(progress, false);
            upload.IsHitTestVisible = true;
        }
        #endregion
    }
}
