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
using MaterialDesignThemes.Wpf;
using System.Collections.ObjectModel;

namespace MystatDesktopWpf.UserControls.DialogContent
{
    /// <summary>
    /// Interaction logic for DownloadUploadedHomework.xaml
    /// </summary>
    public partial class DonwloadHomeworkPreview : UserControl
    {
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(DonwloadHomeworkPreview));
        public string Header
        {
            get => (string)GetValue(HeaderProperty);
            set => SetValue(HeaderProperty, value);
        }

        public static readonly DependencyProperty TextBoxHintProperty =
            DependencyProperty.Register("TextBoxHint", typeof(string), typeof(DonwloadHomeworkPreview));
        public string TextBoxHint
        {
            get => (string)GetValue(TextBoxHintProperty);
            set => SetValue(TextBoxHintProperty, value);
        }

        public string Text { set => commentTextBox.Text = value; }
        public bool IsFileMissing { set => downloadButton.Visibility = value ? Visibility.Collapsed : Visibility.Visible; }

        public DonwloadHomeworkPreview()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
