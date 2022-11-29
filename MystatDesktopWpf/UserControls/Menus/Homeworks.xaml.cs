﻿using MystatAPI.Entity;
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
using MystatDesktopWpf.UserControls;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for Homeworks.xaml
    /// </summary>
    public partial class Homeworks : UserControl
    {
        HomeworksViewModel viewModel;
        static HttpClient httpClient = new();

        UploadHomeworkDialogContent uploadContent = new();
        DeleteHomeworkDialogContent deleteContent = new();
        public Homeworks()
        {
            InitializeComponent();

            viewModel = (HomeworksViewModel)FindResource("HomeworksViewModel");
            uploadContent.Host = homeworkDialog;
        }

        public async void DownloadHomework(Homework homework)
        {
            try
            {
                var res = await httpClient.GetAsync(homework.FilePath);
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

        public async void OpenUploadDialog(Homework homework, ICollection<Homework> source, Button progress, Button upload, string[]? files = null)
        {
            uploadContent.ResetContent();
            uploadContent.Homework = homework;
            uploadContent.HomeworkSource = source;
            uploadContent.Files = files;
            bool? result = (bool?)await homeworkDialog.ShowDialog(uploadContent);

            if(result.HasValue && result == true)
            {
                // Делаем кнопку загрузки крутиться
                ButtonProgressAssist.SetIsIndicatorVisible(progress, true);
                upload.IsHitTestVisible = false;
                try
                {
                    if (uploadContent.Files != null && uploadContent.Archive)
                    {
                        // Архивация
                        using MemoryStream stream = new();
                        using (ZipArchive zip = new(stream, ZipArchiveMode.Create, true))
                        {
                            foreach (var path in uploadContent.Files)
                                zip.CreateEntryFromAny(path);
                        }
                        HomeworkFile file = new(uploadContent.ArchiveName, stream.ToArray());

                        await MystatAPISingleton.mystatAPIClient.UploadHomeworkFile(uploadContent.Homework.Id, file, uploadContent.Comment);
                    }
                    else
                        await MystatAPISingleton.mystatAPIClient.UploadHomework(uploadContent.Homework.Id, uploadContent.Files?[0], uploadContent.Comment);

                    uploadContent.Homework.Status = HomeworkStatus.Uploaded;
                    uploadContent.HomeworkSource.Remove(uploadContent.Homework);
                    viewModel.Uploaded.Add(uploadContent.Homework);
                    
                    string workUploaded = (string)FindResource("m_WorkUploaded");
                    snackbar.MessageQueue?.Enqueue(workUploaded);
                }
                catch (Exception)
                {
                    string workUploadError = (string)FindResource("m_WorkUploadError");
                    snackbar.MessageQueue?.Enqueue(workUploadError);
                }
                ButtonProgressAssist.SetIsIndicatorVisible(progress, false);
                upload.IsHitTestVisible = true;
            }
        }

        public async void OpenDeleteDialog(Homework homework)
        {
            bool? result = (bool?)await homeworkDialog.ShowDialog(deleteContent);

            if (result.HasValue && result == true)
            {
                try
                {
                    if (await MystatAPISingleton.mystatAPIClient.RemoveHomework(homework.Id) == false)
                        throw new HttpRequestException("Error deleting homework");

                    viewModel.Uploaded.Remove(homework);
                    if (DateTime.Parse(homework.OverdueTime) < DateTime.Now)
                    {
                        homework.Status = HomeworkStatus.Overdue;
                        viewModel.Overdue.Add(homework);
                    }
                    else
                    {
                        homework.Status = HomeworkStatus.Active;
                        viewModel.Active.Add(homework);
                    }

                    string workDeleted = (string)FindResource("m_WorkDeleted");
                    snackbar.MessageQueue?.Enqueue(workDeleted);
                }
                catch (Exception)
                {
                    string deleteWorkError = (string)FindResource("m_DeleteWorkError");
                    snackbar.MessageQueue?.Enqueue(deleteWorkError);
                }
            }
        }
    }
}
