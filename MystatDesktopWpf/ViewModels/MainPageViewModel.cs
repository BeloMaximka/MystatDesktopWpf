using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MystatDesktopWpf.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
        public bool LoadingExams { get; private set; }
        public async Task LoadFutureExams()
        {
            if (LoadingExams) return;
            LoadingExams = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetFutureExams();
                    FutureExams = new(result);
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingExams = false;
        }

        public bool LoadingActivities { get; private set; }
        public async Task LoadActivities()
        {
            if (LoadingActivities) return;
            LoadingActivities = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetActivities();
                    List<OptimizedActiviy> optimizedActivies = new();
                    foreach (var activityLog in result)
                    {
                        foreach (var activity in activityLog.Activity)
                        {
                            optimizedActivies.Add(new OptimizedActiviy(activity));
                            if (activity.Badge > 0) optimizedActivies.Add(new OptimizedActiviy(activity, true));
                        }
                    }

                    Activities = new(optimizedActivies);
                    break;
                }
                catch (Exception e)
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingActivities = false;
        }

        public bool LoadingLeaders { get; private set; }
        public async Task LoadLeaders()
        {
            if (LoadingLeaders) return;
            LoadingLeaders = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetGroupLeaders();
                    GroupLeaders = new(result);
                    result = await MystatAPISingleton.Client.GetStreamLeaders();
                    StreamLeaders = new(result);
                    break;
                }
                catch (Exception e)
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingLeaders = false;
        }

        public bool LoadingSummary { get; private set; }
        public async Task LoadSummary()
        {
            if (LoadingSummary) return;
            LoadingSummary = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetGroupLeadersSummary();
                    GroupPosition = result.Position.GetValueOrDefault(-1);
                    result = await MystatAPISingleton.Client.GetStreamLeadersSummary();
                    StreamPosition = result.Position.GetValueOrDefault(-1);
                    var resultSecond = await MystatAPISingleton.Client.GetGradesInfo();
                    AverageGrade = resultSecond[^1].Points.GetValueOrDefault(-1);
                    resultSecond = await MystatAPISingleton.Client.GetAttendanceInfo();
                    Attendance = resultSecond[^1].Points.GetValueOrDefault(-1);
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingSummary = false;
        }

        public bool LoadingHomeworkInfo { get; private set; }
        public async Task LoadHomeworkInfo()
        {
            if (LoadingHomeworkInfo) return;
            LoadingHomeworkInfo = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetHomeworkCount();
                    HomeworkChecked = result[0].Count;
                    HomeworkCurrent = result[1].Count;
                    HomeworkOverdue = result[2].Count;
                    HomeworkUploaded = result[3].Count;
                    HomeworkDeleted = result[4].Count;
                    HomeworkTotal = result[5].Count;
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
            LoadingHomeworkInfo = false;
        }

        private int homeworkTotal;
        public int HomeworkTotal { get => homeworkTotal; set => SetProperty(ref homeworkTotal, value); }

        private int homeworkCurrent;
        public int HomeworkCurrent { get => homeworkCurrent; set => SetProperty(ref homeworkCurrent, value); }

        private int homeworkUploaded;
        public int HomeworkUploaded { get => homeworkUploaded; set => SetProperty(ref homeworkUploaded, value); }

        private int homeworkChecked;
        public int HomeworkChecked { get => homeworkChecked; set => SetProperty(ref homeworkChecked, value); }

        private int homeworkOverdue;
        public int HomeworkOverdue { get => homeworkOverdue; set => SetProperty(ref homeworkOverdue, value); }

        private int homeworkDeleted;
        public int HomeworkDeleted { get => homeworkDeleted; set => SetProperty(ref homeworkDeleted, value); }

        private int groupPosition = -1;
        public int GroupPosition { get => groupPosition; set => SetProperty(ref groupPosition, value); }

        private int streamPosition = -1;
        public int StreamPosition { get => streamPosition; set => SetProperty(ref streamPosition, value); }

        private int averageGrade = -1;
        public int AverageGrade { get => averageGrade; set => SetProperty(ref averageGrade, value); }

        private int attendance = -1;
        public int Attendance { get => attendance; set => SetProperty(ref attendance, value); }

        private ObservableCollection<OptimizedActiviy> activities = new();
        public ObservableCollection<OptimizedActiviy> Activities { get => activities; set => SetProperty(ref activities, value); }

        private ObservableCollection<Exam> futureExams = new();
        public ObservableCollection<Exam> FutureExams { get => futureExams; set => SetProperty(ref futureExams, value); }

        private ObservableCollection<Student> groupLeaders = new();
        public ObservableCollection<Student> GroupLeaders { get => groupLeaders; set => SetProperty(ref groupLeaders, value); }

        private ObservableCollection<Student> streamLeaders = new();
        public ObservableCollection<Student> StreamLeaders { get => streamLeaders; set => SetProperty(ref streamLeaders, value); }
    }

    internal class OptimizedActiviy
    {
        private static readonly Dictionary<sbyte, string> achievementsNames =
        new(){
            { -1, "m_PurchaseInMarket" },
            { 1, "m_5Visits" },
            { 2, "m_10Visits" },
            { 3, "m_20Visits" },
            { 4, "m_5VisitsIntime" },
            { 5, "m_10VisitsIntime" },
            { 6, "m_20VisitsIntime" },
            { 7, "m_PairVisit" },
            { 8, "m_HomeworkRedo" },
            { 9, "m_HomeworkIntime" },
            { 10, "m_LaboratoryIntime" },
            { 11, "m_ClassworkReward" },
            { 14, "m_FullProfile" },
            { 17, "m_QuizParticipation" },
            { 20, "m_Grade" },
            { 23, "m_EmailConfirm" },
            { 28, "m_HomeworkExpired" },
            { 30, "m_LessonRate" },
        };

        private static readonly PackIconKind[] icons =
        {
            PackIconKind.Diamond,
            PackIconKind.AlphaICircle,
            PackIconKind.Prize
        };

        private readonly sbyte id;
        private readonly int points;
        private readonly byte iconType;
        public string Date { get; private set; }
        public string Points { get => points.ToString("+0;-#"); }
        public PackIconKind Icon { get => icons[iconType]; }
        public string Name
        {
            get
            {
                achievementsNames.TryGetValue(id, out var name);
                name ??= "m_Unknown";
                string result = (string)App.Current.FindResource(name);
                return result;
            }
        }

        public OptimizedActiviy(Activity activity, bool badge = false)
        {
            id = (sbyte)activity.AchievementsId;
            if (badge)
            {
                points = activity.Badge;
                iconType = 2;
            }
            else
            {
                points = activity.CurrentPoint;
                if (activity.Action == 0) points *= -1;
                iconType = (byte)(activity.PointTypesName == "DIAMOND" ? 0 : 1);
            }
            Date = DateTime.Parse(activity.Date).ToString("d");
        }
    }
}
