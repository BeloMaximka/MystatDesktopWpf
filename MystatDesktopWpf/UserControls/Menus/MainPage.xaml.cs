using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.ViewModels;
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

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl
    {
        MainPageViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            DataContext = viewModel;
            classRadioButton.Checked += ClassRadioButton_Checked;
            groupRadioButton.Checked += GroupRadioButton_Checked;
        }

        private void ClassRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            leadersList.ItemsSource = viewModel.StreamLeaders;
        }

        private void GroupRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            leadersList.ItemsSource = viewModel.GroupLeaders;
        }

        private void studentName_Initialized(object sender, EventArgs e)
        {
            Run run = (Run)sender;
            Student student = (Student)run.DataContext;
            if (student.Id == MystatAPISingleton.Profile.Id)
                run.FontWeight = FontWeights.Bold;
        }

        private void ItemsControl_TargetUpdated(object sender, DataTransferEventArgs e)
        {
            ItemsControl control = (ItemsControl)sender;
            noExamsTextBlock.Visibility = control.Items.Count == 0 ? Visibility.Visible : Visibility.Collapsed;
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UserControl control = (UserControl)sender;
            mainGrid.Columns = control.ActualWidth < 1300 ? 2 : 3;
        }
    }
}
