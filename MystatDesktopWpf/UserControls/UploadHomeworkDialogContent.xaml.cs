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
using MaterialDesignThemes.Wpf;
using System.Windows.Threading;
using MystatAPI.Entity;
using System.Text.RegularExpressions;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for UploadHomeworkDialogContent.xaml
    /// </summary>
    public partial class UploadHomeworkDialogContent : UserControl
    {
        public string[]? Files { get; private set; }
        public string? Comment { get; private set; }
        public bool Archive { get; private set; } = false;
        public string ArchiveName { get => fileTextBox.Text + zipTextBlock.Text; }
        public Homework Homework { get; set; }
        public ICollection<Homework> HomeworkSource { get; set; }
        public DialogHost? Host { get; set; }
        public event Action? UploadConfirm;

        DispatcherTimer clickAwayDebounce = new();

        public UploadHomeworkDialogContent()
        {
            InitializeComponent();
            clickAwayDebounce.Interval = TimeSpan.FromMilliseconds(300);
            clickAwayDebounce.Tick += EnableClickAway; ;
        }

        public void ResetContent()
        {
            Files = null;
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
            if (Files?.Length > 0)
            {
                dropDownCard.Visibility = Visibility.Collapsed;
                fileLine.Visibility = Visibility.Visible;

                string extension = Path.GetExtension(Files[0]);
                if (Files?.Length != 1 || extension == ".txt" || extension == ".csv")
                {
                    Archive = true;
                    fileTextBox.Visibility = Visibility.Visible;
                    zipTextBlock.Visibility = Visibility.Visible;
                    fileTextBox.Text = "Homework";
                }
                else
                {
                    Archive = false;
                    regularFileTextBlock.Visibility = Visibility.Visible;
                    regularFileTextBlock.Text = Path.GetFileName(Files[0]);
                }
            }
        }
        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = "";
        }

        void dropDownCard_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Files = (string[])e.Data.GetData(DataFormats.FileDrop);
                UpdateFileInfo();
            }
        }

        void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            Files = null;
            dropDownCard.Visibility = Visibility.Visible;

            fileLine.Visibility = Visibility.Collapsed;
            regularFileTextBlock.Visibility = Visibility.Collapsed;
            fileTextBox.Visibility = Visibility.Collapsed;
            zipTextBlock.Visibility = Visibility.Collapsed;
        }

        void openExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            if (Host != null)
                Host.CloseOnClickAway = false;

            OpenFileDialog dialog = new() { Multiselect = true };
            if (dialog.ShowDialog() ?? false)
            {
                Files = dialog.FileNames;
                UpdateFileInfo();
            }
            clickAwayDebounce.Start();
        }

        void EnableClickAway(object? sender, EventArgs e)
        {
            if (Host != null)
                Host.CloseOnClickAway = true;
            clickAwayDebounce.Stop();
        }

        void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (Files == null && textBox.Text == string.Empty)
            {
                errorTextBlock.Text = "Upload the file or write a comment";
                errorTextBlock.Visibility = Visibility.Visible;
                return;
            }
            if (Archive && fileTextBox.Text.Length == 0)
            {
                errorTextBlock.Text = "Archive name cannot be empty";
                errorTextBlock.Visibility = Visibility.Visible;
                return;
            }

            UploadConfirm?.Invoke();
        }

        private void fileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Чтобы нельзя было вставить <>:"/\|?* в название файла
            // PreviewTextInput не подходит, ибо он пропускает вставку символов
            TextBox textBox = (TextBox)sender;
            if ("<>:\"/\\|?*".Any(textBox.Text.Contains))
                textBox.Text = Regex.Replace(textBox.Text, "[\"<>:\\\"\\/\\\\|?*\"]", "");
        }
    }
}
