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

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for Homeworks.xaml
    /// </summary>
    public partial class Homeworks : UserControl, IHomeworkManager
    {
        static HttpClient httpClient = new();

        public Homeworks()
        {
            InitializeComponent();
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
    }
}
