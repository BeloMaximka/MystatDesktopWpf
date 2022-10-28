using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal static class ScheduleNotificationService
    {
        static List<DayScheduleForNotification> TodaySchedule;
        static int notificationDelay = 10;
        static bool notificationsConfigured = false;
        static Random rand = new Random();

        public static event Action<DaySchedule, int>? OnTimerElapsed;
        public static event Action? OnTimersConfigured;

        static ScheduleNotificationService()
        {
            TodaySchedule = new();
        }

        public static async Task Configure(bool notificationsWithDelay = false)
        {
            if (notificationsConfigured)
            {
                DisableAllNotifications();
                TaskService.CancelTask("daily-schedule-reload");
                notificationsConfigured = false;
            }

            var result = await LoadSchedule();

            if (!result) return;

            EnableAllNotifications(notificationsWithDelay);

            var nextDay = DateTime.Now.AddDays(1);
            TaskService.ScheduleTask("daily-schedule-reload", nextDay, new TimeOnly(1, 0), async () => await Configure(notificationsWithDelay));
            OnTimersConfigured?.Invoke();
        }

        public static async Task<bool> LoadSchedule()
        {
            TodaySchedule.Clear();

            try
            {
                var response = await MystatAPISingleton.mystatAPIClient.GetScheduleByDate(DateTime.Now);

                if (response is null) return false;
                TodaySchedule = response.Select(i => new DayScheduleForNotification(i)).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void EnableAllNotifications(bool withDelay = false)
        {
            foreach (var item in TodaySchedule)
            {
                SetNotification(item, withDelay);
            }

            notificationsConfigured = true;
        }

        static bool SetNotification(DayScheduleForNotification item, bool withDelay)
        {
            if (!item.IsNotificationEnabled) return false;

            var time = TimeOnly.Parse(item.DaySchedule.StartedAt).ToTimeSpan();
            time = withDelay ? time.Subtract(TimeSpan.FromMinutes(notificationDelay)) : time;
            string timerId = CreateId(item.DaySchedule);
            return TaskService.ScheduleTask(timerId, time, () => OnTimerElapsed?.Invoke(item.DaySchedule, notificationDelay));
        }

        public static bool EnableNotification(DaySchedule enableForItem, bool withDelay = false)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == enableForItem.StartedAt);

            if (item is null)
            {
                return false;
            }

            item.IsNotificationEnabled = true;
            return SetNotification(item, withDelay);
        }

        public static bool DisableNotification(DaySchedule cancelForItem)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == cancelForItem.StartedAt);

            if(item is null)
            {
                return false;
            }

            return DisableNotification(item);
        }

        static bool DisableNotification(DayScheduleForNotification cancelForItem)
        {
            var ids = TaskService.TimersIds.Where(id => id.StartsWith(cancelForItem.DaySchedule.StartedAt)).ToArray();

            if (ids.Length == 0 || !cancelForItem.IsNotificationEnabled) return false;

            foreach (var id in ids)
            {
                if (!TaskService.CancelTask(id))
                {
                    return false;
                }
            }

            cancelForItem.IsNotificationEnabled = false;
            return true;
        }

        public static void DisableAllNotifications()
        {
            foreach (var item in TodaySchedule)
            {
                DisableNotification(item);
            }
        }

        public static void SetNotificationDelay(int value)
        {
            if(value > 0)
            {
                notificationDelay = value;
            }
        }

        static string CreateId(DaySchedule daySchedule)
        {
            return $"{daySchedule.StartedAt}_{rand.Next(100)}";
        }
    }

    internal class DayScheduleForNotification
    {
        public DaySchedule DaySchedule { get; set; }
        public bool IsNotificationEnabled { get; set; }

        public DayScheduleForNotification(DaySchedule daySchedule, bool isNotificationEnabled = true)
        {
            DaySchedule = daySchedule;
            IsNotificationEnabled = isNotificationEnabled;
        }
    }
}
