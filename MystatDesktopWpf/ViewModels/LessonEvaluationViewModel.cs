using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace MystatDesktopWpf.ViewModels
{
    internal class LessonEvaluationViewModel : ViewModelBase
    {
        private ObservableCollection<EvaluateLessonItemWithMark> lessons = new();
        public ObservableCollection<EvaluateLessonItemWithMark> Lessons { get => lessons; set => SetProperty(ref lessons, value); }

        public bool LoadingLessons { get; private set; }
        public async Task LoadLessons()
        {
            if (LoadingLessons) return;
            LoadingLessons = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetEvaluateLessonList();
                    List<EvaluateLessonItemWithMark> lessons = new();
                    foreach (var item in result)
                    {
                        lessons.Add(new EvaluateLessonItemWithMark(item));
                    }
                    Lessons = new(lessons);
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingLessons = false;
        }

        public async Task EvaluateLesson(EvaluateLessonItemWithMark item)
        {
            if (await MystatAPISingleton.Client.EvaluateLesson(item.Key, item.LessonMark, item.TeacherMark, item.Comment))
            {
                Lessons.Remove(item);
            }
        }

        public static async Task AutoEvaluateAllLessons()
        {
            var result = await MystatAPISingleton.Client.GetEvaluateLessonList();
            foreach (var item in result)
            {
                await MystatAPISingleton.Client.EvaluateLesson(item.Key, 5, 5);
            }
        }
        
    }

    internal class EvaluateLessonItemWithMark : EvaluateLessonItem
    {
        public EvaluateLessonItemWithMark(EvaluateLessonItem item)
        {
            VisitDate = item.VisitDate;
            TeacherFullName = item.TeacherFullName;
            Key = item.Key;
            SpecName = item.SpecName;
        }
        public int LessonMark { get; set; } = 0;
        public int TeacherMark { get; set; } = 0;
        public string? Comment { get; set; }
    }
}