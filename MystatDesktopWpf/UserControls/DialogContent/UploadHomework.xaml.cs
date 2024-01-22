using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using MystatAPI.Entity;
using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace MystatDesktopWpf.UserControls.DialogContent
{
    /// <summary>
    /// Interaction logic for UploadHomeworkDialogContent.xaml
    /// </summary>
    public partial class UploadHomework : UserControl
    {
        public static string[] mystatForbiddenExtentions = new string[] { ".txt", ".csv", ".py" };
        public static readonly DependencyProperty HeaderProperty =
           DependencyProperty.Register("Header", typeof(string), typeof(UploadHomework));
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }
        public static readonly DependencyProperty SendButtonNameProperty =
           DependencyProperty.Register("SendButtonName", typeof(string), typeof(UploadHomework));
        public string SendButtonName
        {
            get => (string)GetValue(SendButtonNameProperty);
            set => SetValue(SendButtonNameProperty, value);
        }

        private string[]? files;
        public string[]? Files
        {
            get => files;
            set
            {
                files = value;
                UpdateFileInfo();
            }
        }
        public string? Comment { get => textBox.Text; }
        public bool Archive { get; private set; } = false;
        public string ArchiveName { get => fileTextBox.Text + zipTextBlock.Text; }
        public Homework Homework { get; set; }
        public ICollection<Homework> HomeworkSource { get; set; }
        public DialogHost? Host { get; set; }

        private readonly DispatcherTimer clickAwayDebounce = new();

        public UploadHomework()
        {
            InitializeComponent();
            clickAwayDebounce.Interval = TimeSpan.FromMilliseconds(300);
            clickAwayDebounce.Tick += EnableClickAway; ;
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

        private void UpdateFileInfo()
        {
            if (files?.Length > 0)
            {
                dropDownCard.Visibility = Visibility.Collapsed;
                fileLine.Visibility = Visibility.Visible;

                string extension = Path.GetExtension(files[0]);
                if (
                    files?.Length != 1 
                    || (mystatForbiddenExtentions.Contains(extension) && !SettingsService.Settings.Experimental.BypassUploadRestrictions) 
                    || Directory.Exists(files[0]))
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
                    regularFileTextBlock.Text = Path.GetFileName(files[0]);
                }
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBox.Text))
                textBox.Text = "";
        }

        private void DropDownCard_Drop(object sender, DragEventArgs e)
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

        private void OpenExplorerButton_Click(object sender, RoutedEventArgs e)
        {
            if (Host != null)
                Host.CloseOnClickAway = false;

            OpenFileDialog dialog = new() { Multiselect = true };
            if (dialog.ShowDialog() ?? false)
            {
                files = dialog.FileNames;
                UpdateFileInfo();
            }
            clickAwayDebounce.Start();
        }

        private void EnableClickAway(object? sender, EventArgs e)
        {
            if (Host != null)
                Host.CloseOnClickAway = true;
            clickAwayDebounce.Stop();
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            if (files == null && textBox.Text == string.Empty)
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
            DialogHost.CloseDialogCommand.Execute(true, Host);
        }

        private void FileTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Чтобы нельзя было вставить <>:"/\|?* в название файла
            // PreviewTextInput не подходит, ибо он пропускает вставку символов
            TextBox textBox = (TextBox)sender;
            if ("<>:\"/\\|?*".Any(textBox.Text.Contains))
                textBox.Text = Regex.Replace(textBox.Text, "[\"<>:\\\"\\/\\\\|?*\"]", "");
        }
    }
}
