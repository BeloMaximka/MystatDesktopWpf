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
using MystatDesktopWpf.Domain;

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

        public static readonly DependencyProperty HomeworkManagerProperty =
            DependencyProperty.Register("HomeworkManager", typeof(IHomeworkManager), typeof(UserControl));
        public IHomeworkManager? HomeworkManager
        {
            get => (IHomeworkManager)GetValue(HomeworkManagerProperty);
            set => SetValue(HomeworkManagerProperty, value);
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

        private void Card_Initialized(object sender, EventArgs e)
        {
            return;
        }

        private void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            HomeworkManager?.DownloadHomework((Homework)((Button)sender).Tag);
        }
    }
}
