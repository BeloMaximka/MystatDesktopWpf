using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : UserControl, IRefreshable
    {
        private readonly MainPageViewModel viewModel;
        public MainPage()
        {
            InitializeComponent();
            viewModel = new MainPageViewModel();
            DataContext = viewModel;
            classRadioButton.Checked += ClassRadioButton_Checked;
            groupRadioButton.Checked += GroupRadioButton_Checked;
            Refresh();
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

        public void Refresh()
        {
            LoadSummary();
            LoadActivities();
            LoadLeaders();
            LoadFutureExams();
            LoadHomeworkInfo();
        }

        private async void LoadSummary()
        {
            if (viewModel.LoadingSummary) return;
            summaryProgress.Visibility = Visibility.Visible;
            await viewModel.LoadSummary();
            summaryProgress.Visibility = Visibility.Collapsed;
        }
        private async void LoadActivities()
        {
            if (viewModel.LoadingActivities) return;
            activityProgress.Visibility = Visibility.Visible;
            await viewModel.LoadActivities();
            activityProgress.Visibility = Visibility.Collapsed;
        }
        private async void LoadLeaders()
        {
            if (viewModel.LoadingLeaders) return;
            leadersProgress.Visibility = Visibility.Visible;
            await viewModel.LoadLeaders();
            leadersProgress.Visibility = Visibility.Collapsed;
        }
        private async void LoadFutureExams()
        {
            if (viewModel.LoadingExams) return;
            examinationProgress.Visibility = Visibility.Visible;
            await viewModel.LoadFutureExams();
            examinationProgress.Visibility = Visibility.Collapsed;
        }
        private async void LoadHomeworkInfo()
        {
            if (viewModel.LoadingHomeworkInfo) return;
            homeworkProgress.Visibility = Visibility.Visible;
            await viewModel.LoadHomeworkInfo();
            homeworkProgress.Visibility = Visibility.Collapsed;
        }
    }
}
