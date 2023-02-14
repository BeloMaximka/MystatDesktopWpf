using MaterialDesignThemes.Wpf;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls.Menus
{
    /// <summary>
    /// Interaction logic for LessonEvaluation.xaml
    /// </summary>
    public partial class LessonEvaluation : UserControl, IRefreshable
    {
        private readonly LessonEvaluationViewModel viewModel = new();
        public LessonEvaluation()
        {
            DataContext = viewModel;

            InitializeComponent();
            Refresh();
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
                return; // temp
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
    }
}