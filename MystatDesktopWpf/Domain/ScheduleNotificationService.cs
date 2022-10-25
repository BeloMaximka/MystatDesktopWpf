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

        public static event Action<DaySchedule, int>? OnTimerElapsed;
        public static event Action? OnTimersConfigured;

        static ScheduleNotificationService()
        {
            TodaySchedule = new();
        }

        public static async Task Configure()
        {
            if (notificationsConfigured)
            {
                DisableAllNotifications();
                TaskService.CancelTask("daily-schedule-reload");
                notificationsConfigured = false;
            }

            var result = await LoadSchedule();

            if (!result) return;

            EnableAllNotifications();

            var nextDay = DateTime.Now.AddDays(1);
            TaskService.ScheduleTask("daily-schedule-reload", nextDay, new TimeOnly(1, 0), async () => await Configure());
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

        public static void EnableAllNotifications()
        {
            foreach (var item in TodaySchedule)
            {
                SetNotification(item);
            }

            notificationsConfigured = true;
        }

        static bool SetNotification(DayScheduleForNotification item)
        {
            if (!item.IsNotificationEnabled) return false;

            var time = TimeOnly.Parse(item.DaySchedule.StartedAt).ToTimeSpan();
            time = time.Subtract(TimeSpan.FromMinutes(notificationDelay));
            return TaskService.ScheduleTask(item.DaySchedule.StartedAt, time, () => OnTimerElapsed?.Invoke(item.DaySchedule, notificationDelay));
        }

        public static bool EnableNotification(DaySchedule enableForItem)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == enableForItem.StartedAt);

            if (item is null)
            {
                return false;
            }

            item.IsNotificationEnabled = true;
            return SetNotification(item);
        }

        public static bool DisableNotification(DaySchedule cancelForItem)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == cancelForItem.StartedAt);

            if(item is null)
            {
                return false;
            }

            item.IsNotificationEnabled = false;
            return TaskService.CancelTask(cancelForItem.StartedAt);
        }

        static bool DisableNotification(DayScheduleForNotification cancelForItem)
        {
            cancelForItem.IsNotificationEnabled = false;
            return TaskService.CancelTask(cancelForItem.DaySchedule.StartedAt);
        }

        public static void DisableAllNotifications()
        {
            foreach (var item in TodaySchedule)
            {
                DisableNotification(item);
            }
        }

        public static async Task SetNotificationDelay(int value)
        {
            if(value > 0)
            {
                notificationDelay = value;

                if(notificationsConfigured)
                {
                    await Configure();
                }
            }
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
