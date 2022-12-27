using MaterialDesignThemes.Wpf;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.ViewModels
{
    internal class MainPageViewModel : ViewModelBase
    {
        
        public MainPageViewModel()
        {
            LoadActivities();
            LoadLeaders();
            LoadSummary();
            LoadFutureExams();
            LoadHomeworkInfo();
        }

        public async void LoadFutureExams()
        {
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
        }

        public async void LoadActivities()
        {
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetActivities();
                    List<OptimizedActiviy> optimizedActivies = new();
                    foreach (var activity in result)
                    {
                        optimizedActivies.Add(new OptimizedActiviy(activity));
                        if (activity.Badge > 0) optimizedActivies.Add(new OptimizedActiviy(activity, true));
                    }

                    Activities = new(optimizedActivies);
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
        }

        public async void LoadLeaders()
        {
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
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
        }

        public async void LoadSummary()
        {
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    var result = await MystatAPISingleton.Client.GetGroupLeadersSummary();
                    GroupPosition = result.Position.Value;
                    result = await MystatAPISingleton.Client.GetStreamLeadersSummary();
                    StreamPosition = result.Position.Value;
                    var resultSecond = await MystatAPISingleton.Client.GetGradesInfo();
                    AverageGrade = resultSecond[^1].Points.Value;
                    resultSecond = await MystatAPISingleton.Client.GetAttendanceInfo();
                    Attendance = resultSecond[^1].Points.Value;
                    break;
                }
                catch
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                }
            }
        }

        public async void LoadHomeworkInfo()
        {
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
        }

        int homeworkTotal;
        public int HomeworkTotal { get => homeworkTotal; set => SetProperty(ref homeworkTotal, value); }

        int homeworkCurrent;
        public int HomeworkCurrent { get => homeworkCurrent; set => SetProperty(ref homeworkCurrent, value); }

        int homeworkUploaded;
        public int HomeworkUploaded { get => homeworkUploaded; set => SetProperty(ref homeworkUploaded, value); }

        int homeworkChecked;
        public int HomeworkChecked { get => homeworkChecked; set => SetProperty(ref homeworkChecked, value); }
        
        int homeworkOverdue;
        public int HomeworkOverdue { get => homeworkOverdue; set => SetProperty(ref homeworkOverdue, value); }

        int homeworkDeleted;
        public int HomeworkDeleted { get => homeworkDeleted; set => SetProperty(ref homeworkDeleted, value); }

        int groupPosition;
        public int GroupPosition { get => groupPosition; set => SetProperty(ref groupPosition, value); }

        int streamPosition;
        public int StreamPosition { get => streamPosition; set => SetProperty(ref streamPosition, value); }

        int averageGrade;
        public int AverageGrade { get => averageGrade; set => SetProperty(ref averageGrade, value); }

        int attendance;
        public int Attendance { get => attendance; set => SetProperty(ref attendance, value); }

        ObservableCollection<OptimizedActiviy> activities = new();
        public ObservableCollection<OptimizedActiviy> Activities { get => activities; set => SetProperty(ref activities, value); }

        ObservableCollection<Exam> futureExams = new();
        public ObservableCollection<Exam> FutureExams { get => futureExams; set => SetProperty(ref futureExams, value); }

        ObservableCollection<Student> groupLeaders = new();
        public ObservableCollection<Student> GroupLeaders { get => groupLeaders; set => SetProperty(ref groupLeaders, value); }

        ObservableCollection<Student> streamLeaders = new();
        public ObservableCollection<Student> StreamLeaders { get => streamLeaders; set => SetProperty(ref streamLeaders, value); }
    }

    class OptimizedActiviy
    {
        static readonly Dictionary<byte, string> achievementsNames =
        new(){
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
            { 30, "m_LessonRate" },
        };
        static readonly PackIconKind[] icons =
        {
            PackIconKind.Diamond,
            PackIconKind.AlphaICircle,
            PackIconKind.Prize
        };
        readonly byte id;
        readonly sbyte points;
        readonly byte iconType;
        public string Date { get; private set; }
        public string Points { get => points.ToString("+0;-#"); }
        public PackIconKind Icon { get => icons[iconType]; }
        public string Name 
        { 
            get
            {
                string result = (string)App.Current.FindResource(achievementsNames[id]);
                if (id == 20) result += $": {7 + points}";
                return result;
            }

        }

        public OptimizedActiviy(Activity activity, bool badge = false)
        {
            id = (byte)activity.AchievementsId;
            if(badge)
            {
                points = (sbyte)activity.Badge;
                iconType = 2;
            }
            else
            {
                points = (sbyte)activity.CurrentPoint;
                if (activity.Action == 0) points *= -1;
                iconType = (byte)(activity.PointTypesName == "DIAMOND" ? 0 : 1);
            }
            
            Date = DateTime.Parse(activity.Date).ToString("d");
        }
    }
}
