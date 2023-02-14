using System;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;

namespace MystatDesktopWpf.ViewModels
{
    internal class LessonEvaluationViewModel : ViewModelBase, INotificationCount
    {
        public LessonEvaluationViewModel()
        {
            LoadLessons();
        }

        private ObservableCollection<EvaluateLessonItemWithMark> lessons = new();
        public ObservableCollection<EvaluateLessonItemWithMark> Lessons { get => lessons; set => SetProperty(ref lessons, value); }

        public bool AutoLessonEvaluationEnabled
        {
            get => SettingsService.Settings.AutoLessonEvaluationEnabled;
            set => SettingsService.SetPropertyValue(nameof(AutoLessonEvaluationEnabled), value);
        }

        public int MenuItemNotifications { get => Lessons.Count; }

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
                    if (SettingsService.Settings.AutoLessonEvaluationEnabled)
                    {
                        foreach (var item in result)
                        {
                            await MystatAPISingleton.Client.EvaluateLesson(item.Key, 5, 5);
                        }
                        return;
                    }
                    List<EvaluateLessonItemWithMark> lessons = new();
                    foreach (var item in result)
                    {
                        lessons.Add(new EvaluateLessonItemWithMark(item));
                    }
                    Lessons = new(lessons);
                    OnPropertyChanged(nameof(MenuItemNotifications));
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
                OnPropertyChanged(nameof(MenuItemNotifications));
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