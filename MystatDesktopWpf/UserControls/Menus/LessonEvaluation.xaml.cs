using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using MystatDesktopWpf.ViewModels;
using System.Linq;
using System;
using System.Windows;
using System.Windows.Controls;
using MystatAPI.Entity;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for LessonEvaluation.xaml
    /// </summary>
    public partial class LessonEvaluation : UserControl, IRefreshable
    {
        private LessonEvaluationViewModel viewModel;
        public LessonEvaluation()
        {
            InitializeComponent();
        }

        private async void AutoRefresh()
        {
            Refresh();
            TaskService.CancelTask("lesson-evaluation-refresh");
            try
            {
                var result = await MystatAPISingleton.Client.GetScheduleByDate(DateTime.Now.AddDays(1));
                if (result.Length > 0)
                {
                    var time = TimezoneConvertionService.Convert(result.Last().FinishedAt).AddMinutes(5);
                    TaskService.ScheduleTask("lesson-evaluation-refresh", DateTime.Now.AddDays(1), TimeOnly.FromDateTime(time), AutoRefresh);
                    return;
                }
            }
            catch { }
            TaskService.ScheduleTask("lesson-evaluation-refresh", DateTime.Now.AddDays(1), new TimeOnly(2, 0), AutoRefresh);
        }

        public async void Refresh()
        {
            if (viewModel.LoadingLessons) return;
            LoadingProgress.Visibility = Visibility.Visible;
            await viewModel.LoadLessons();
            LoadingProgress.Visibility = Visibility.Collapsed;
        }

        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Tag is EvaluateLessonItemWithMark item &&
                button.Parent is FrameworkElement interal &&
                interal.Parent is FrameworkElement parent &&
                parent.FindName("ErrorText") is TextBlock errorText)
            {
                if (item.LessonMark == 0 || item.TeacherMark == 0)
                {
                    errorText.SetResourceReference(TextBlock.TextProperty, "m_BothEvaluationRequired");
                    errorText.Visibility = Visibility.Visible;
                    return;
                }

                errorText.Visibility = Visibility.Collapsed;
                ButtonProgressAssist.SetIsIndicatorVisible(button, true);
                try
                {
                    await viewModel.EvaluateLesson(item);
                }
                catch
                {
                    errorText.SetResourceReference(TextBlock.TextProperty, "m_EvaluationError");
                    errorText.Visibility = Visibility.Visible;

                }
                ButtonProgressAssist.SetIsIndicatorVisible(button, false);
            }
        }

        private void SendWithMaxMarksButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button &&
                button.Parent is FrameworkElement element &&
                element.Parent is FrameworkElement parent &&
                parent.FindName("LessonRatingBar") is RatingBar lessonRatingBar &&
                parent.FindName("TeacherRatingBar") is RatingBar teacherRatingBar)
            {
                lessonRatingBar.Value = 5;
                teacherRatingBar.Value = 5;
            }
            SendButton_Click(sender, e);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is LessonEvaluationViewModel vm)
            {
                viewModel = vm;
                DataContext = vm;
                AutoRefresh();
            }
        }
    }
}