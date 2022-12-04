using MystatAPI.Entity;
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
using System.Windows.Shapes;
using MystatDesktopWpf.UserControls.Menus;

namespace MystatDesktopWpf.UserControls.DialogContent
{
    /// <summary>
    /// Interaction logic for DownloadUploadedHomework.xaml
    /// </summary>
    public partial class DownloadUploadedHomework : UserControl
    {
        public Homeworks HomeworkManager { get; set; }
        Homework homework;
        public Homework Homework {
            get => homework;
            set
            {
                homework = value;
                downloadButton.Visibility = homework.UploadedHomework.FilePath == null ? 
                                            Visibility.Collapsed : Visibility.Visible;
                commentTextBox.Text = homework.UploadedHomework.StudentAnswer;
            } 
        }
        public DownloadUploadedHomework()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager.DownloadHomework(homework.UploadedHomework.FilePath);
        }
    }
}
