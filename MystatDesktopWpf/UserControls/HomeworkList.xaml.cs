using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;
using MaterialDesignThemes.Wpf;
using System.Windows.Automation;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for HomeworkList.xaml
    /// </summary>
    public partial class HomeworkList : UserControl
    {
        // Чтобы можно было прикрутить Binding
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register("Collection", typeof(ObservableCollection<Homework>), typeof(UserControl));
        public ObservableCollection<Homework> Collection
        {
            get => (ObservableCollection<Homework>)GetValue(CollectionProperty);
            set => SetValue(CollectionProperty, value);
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(UserControl));
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public HomeworkList()
        {
            InitializeComponent();
        }

        // Тонкая настройка элементов в зависимости от статуса домашки
        // Чтобы не захламлять и не переусложнять разметку
        private void Card_Initialized(object sender, EventArgs e)
        {
            Card card = (Card)sender;
            HomeworkStatus status = (HomeworkStatus)card.Tag;

            StackPanel completionTime = (StackPanel)card.FindName("completionTime");
            if (status == HomeworkStatus.Checked || status == HomeworkStatus.Uploaded)
                completionTime.Visibility = Visibility.Visible;

            Button deletionComment = (Button)card.FindName("deletionComment");
            if (status == HomeworkStatus.Deleted)
                deletionComment.Visibility = Visibility.Visible;

            ColumnDefinition deleteColumn = (ColumnDefinition)card.FindName("deleteColumn");
            if (status == HomeworkStatus.Deleted || status == HomeworkStatus.Active)
                deleteColumn.Width = new GridLength(0);

            Button uploadButton = (Button)card.FindName("uploadButton");
            if (status == HomeworkStatus.Uploaded || status == HomeworkStatus.Checked)
                ((PackIcon)uploadButton.Content).Kind = PackIconKind.DownloadOutline;

            Button markButton = (Button)card.FindName("markButton");
            Button deleteButton = (Button)card.FindName("deleteButton");
            PackIcon markComment = (PackIcon)card.FindName("markComment");
            if (status != HomeworkStatus.Checked)
            {
                markButton.Visibility = Visibility.Collapsed;
                markComment.Visibility = Visibility.Collapsed;
            }
            else
                ((PackIcon)deleteButton.Content).Kind = PackIconKind.UploadOutline;
        }
    }
}
