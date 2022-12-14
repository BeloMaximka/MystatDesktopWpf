using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatDesktopWpf.UserControls.Menus;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for HomeworkList.xaml
    /// </summary>
    public partial class HomeworkList : UserControl
    {
        private static readonly int defaultPageSize = 6;

        // Чтобы можно было прикрутить Binding
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(HomeworkCollection), typeof(HomeworkList));
        public HomeworkCollection Collection
        {
            get => (HomeworkCollection)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        public static readonly DependencyProperty HomeworkManagerProperty =
            DependencyProperty.Register("HomeworkManager", typeof(Homeworks), typeof(HomeworkList));
        public Homeworks? HomeworkManager
        {
            get => (Homeworks)GetValue(HomeworkManagerProperty);
            set => SetValue(HomeworkManagerProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(HomeworkList));
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public HomeworkList()
        {
            InitializeComponent();
        }

        private void Card_Initialized(object sender, EventArgs e)
        {
            Card card = (Card)sender;
            HomeworkStatus status = (HomeworkStatus)card.Tag;
            if (status == HomeworkStatus.Uploaded || status == HomeworkStatus.Checked)
            {
                Button uploadButton = (Button)card.FindName("uploadButton");
                uploadButton.Click -= UploadButton_Click;
                uploadButton.Click += DownloadUploadedButton_Click;
            }
            if (status == HomeworkStatus.Uploaded)
            {
                Button deleteButton = (Button)card.FindName("deleteButton");
                deleteButton.Click += DeleteButton_Click;
            }
            if (status == HomeworkStatus.Checked)
            {
                Button deleteButton = (Button)card.FindName("deleteButton");
                deleteButton.Click += UploadButton_Click;
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager?.OpenDeleteDialog((Homework)((Button)sender).Tag);
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            Homework homework = (Homework)((Button)sender).Tag;
            HomeworkManager?.DownloadHomework(homework.FilePath);
        }

        private void UploadButton_Click(object sender, RoutedEventArgs e)
        {
            Button uploadButton = (Button)sender;
            Grid grid = (Grid)uploadButton.Parent;
            Button progressButton = (Button)grid.FindName("progressButton");
            HomeworkManager?.OpenUploadDialog((Homework)uploadButton.Tag, Collection.Items, progressButton, uploadButton);
        }

        private void DownloadUploadedButton_Click(object sender, RoutedEventArgs e)
        {
            Button uploadButton = (Button)sender;
            Homework homework = (Homework)uploadButton.Tag;
            if (homework.UploadedHomework.StudentAnswer != null)
                HomeworkManager?.OpenDownloadUploadedDialog(homework);
            else
                HomeworkManager?.DownloadHomework(homework.UploadedHomework.FilePath);
        }

        private void Card_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                Card card = (Card)sender;
                Button uploadButton = (Button)card.FindName("uploadButton");
                Button progressButton = (Button)card.FindName("progressButton");
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                HomeworkManager?.OpenUploadDialog((Homework)uploadButton.Tag, Collection.Items, progressButton, uploadButton, files);
            }
        }

        private void CommentButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string comment = (string)button.Tag;
            popupComment.PlacementTarget = button;
            commentTextBox.Text = comment;
            popupComment.IsOpen = true;
        }

        private async void LoadPageButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            button.IsHitTestVisible = false;
            ButtonProgressAssist.SetIsIndicatorVisible(progressPageButton, true);
            await Collection.LoadNextPage();
            ButtonProgressAssist.SetIsIndicatorVisible(progressPageButton, false);
            button.IsHitTestVisible = true;

            UpdateLoadButtonVisibility();
        }

        public void UpdateLoadButtonVisibility()
        {
            var count = Collection.Items.Count;
            var maxCount = Collection.MaxCount;
            if (count >= maxCount)
            {
                nextPageButton.Visibility = Visibility.Collapsed;
            }
            else if (count >= defaultPageSize)
            {
                nextPageButton.Visibility = Visibility.Visible;
            }
        }
    }
}
