using System;
using System.Collections.Generic;
using System.Linq;
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
using System.IO;
using MystatDesktopWpf.Domain;
using Microsoft.Win32;
using MystatAPI;
using MaterialDesignThemes.Wpf;
using System.IO.Compression;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for UploadHomeworkDialogContent.xaml
    /// </summary>
    public partial class UploadHomeworkDialogContent : UserControl
    {
        string[]? files;
        bool archiveFiles = false;

        public int HomeworkId { get; set; }

        public static readonly DependencyProperty HomeworkManagerProperty =
            DependencyProperty.Register("HomeworkManager", typeof(IHomeworkManager), typeof(UploadHomeworkDialogContent));
        public IHomeworkManager? HomeworkManager
        {
            get => (IHomeworkManager)GetValue(HomeworkManagerProperty);
            set => SetValue(HomeworkManagerProperty, value);
        }

        public UploadHomeworkDialogContent()
        {
            InitializeComponent();
        }

        public UploadHomeworkDialogContent(int homeworkId, IHomeworkManager manager)
        {
            HomeworkId = homeworkId;
            HomeworkManager = manager;
            InitializeComponent();
        }

        public void ResetContent()
        {
            files = null;
            textBox.Text = "";
            dropDownCard.Visibility = Visibility.Visible;

            fileLine.Visibility = Visibility.Collapsed;
            regularFileTextBlock.Visibility = Visibility.Collapsed;
            fileTextBox.Visibility = Visibility.Collapsed;
            zipTextBlock.Visibility = Visibility.Collapsed;
            errorTextBlock.Visibility = Visibility.Collapsed;
        }

        void UpdateFileInfo()
        {
            if(files?.Length > 0)
            {
                dropDownCard.Visibility = Visibility.Collapsed;
                fileLine.Visibility = Visibility.Visible;

                string extension = Path.GetExtension(files[0]);
                if(files?.Length != 1 || extension == ".txt" || extension == ".csv")
                {
                    archiveFiles = true;
                    fileTextBox.Visibility = Visibility.Visible;
                    zipTextBlock.Visibility = Visibility.Visible;
                    fileTextBox.Text = "Homework";
                }
                else
                {
                    archiveFiles = false;
                    regularFileTextBlock.Visibility = Visibility.Visible;
                    regularFileTextBlock.Text = Path.GetFileName(files[0]);
                }
            }
        }
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
               textBox.Text = "";
        }

        private void dropDownCard_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                files = (string[])e.Data.GetData(DataFormats.FileDrop);
                UpdateFileInfo();
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            files = null;
            dropDownCard.Visibility = Visibility.Visible;

            fileLine.Visibility = Visibility.Collapsed;
            regularFileTextBlock.Visibility = Visibility.Collapsed;
            fileTextBox.Visibility = Visibility.Collapsed;
            zipTextBlock.Visibility = Visibility.Collapsed;
        }

        private void openExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new() { Multiselect = true };
            if (dialog.ShowDialog() ?? false)
            {
                files = dialog.FileNames;
                UpdateFileInfo();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if(files == null && textBox.Text == string.Empty)
            {
                errorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            HomeworkManager?.UploadHomework(HomeworkId, files, textBox.Text, archiveFiles, fileTextBox.Text + zipTextBlock.Text);
        }
    }
}
