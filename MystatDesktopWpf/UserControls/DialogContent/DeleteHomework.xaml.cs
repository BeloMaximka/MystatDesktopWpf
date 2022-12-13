using MaterialDesignThemes.Wpf;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls.DialogContent
{
    /// <summary>
    /// Interaction logic for DeleteHomeworkDialogContent.xaml
    /// </summary>
    public partial class DeleteHomework : UserControl
    {
        public DeleteHomework()
        {
            InitializeComponent();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(true, null);
        }
    }
}
