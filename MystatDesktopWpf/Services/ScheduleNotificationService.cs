﻿using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.SubSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Services
{
    internal static class ScheduleNotificationService
    {
        public static List<DayScheduleForNotification> TodaySchedule { get; private set; }
        public static bool OnlyFirstSchedule { get; set; } = true;

        private static readonly Random rand = new();

        public static event Action<DaySchedule, int>? OnTimerElapsed;
        public static event Action? OnTimersConfigured;

        static ScheduleNotificationService()
        {
            TodaySchedule = new();
        }

        public static async Task Configure(int delay, NotificationDelayMode mode)
        {
            await LoadSchedule();
            DisableAllNotifications();
            TaskService.CancelTask("daily-schedule-reconfigure");

            if (TodaySchedule.Count != 0) // чтобы не было исключения при обращении по 0 индексу
            {
                if (OnlyFirstSchedule)
                {
                    if (mode == NotificationDelayMode.Delayed || mode == NotificationDelayMode.Both)
                        EnableNotification(TodaySchedule[0].DaySchedule, delay);
                    if (mode == NotificationDelayMode.WithoutDelay || mode == NotificationDelayMode.Both)
                        EnableNotification(TodaySchedule[0].DaySchedule, 0);
                }
                else
                {
                    if (mode == NotificationDelayMode.Delayed || mode == NotificationDelayMode.Both)
                        EnableAllNotifications(delay);
                    if (mode == NotificationDelayMode.WithoutDelay || mode == NotificationDelayMode.Both)
                        EnableAllNotifications(0);
                }
            }

            var nextDay = DateTime.Now.AddDays(1);
            // Обновляем таймера под новое расписание завтра в 1:00
            TaskService.ScheduleTask("daily-schedule-reconfigure", nextDay, new TimeOnly(1, 0),
                async () => await Configure(delay, SettingsService.Settings.ScheduleNotification.Mode));
            OnTimersConfigured?.Invoke();
        }

        public static async Task<bool> LoadSchedule()
        {
            TodaySchedule.Clear();

            try
            {
                var response = await MystatAPISingleton.Client.GetScheduleByDate(DateTime.Now);

                if (response is null) return false;
                TodaySchedule = response.Select(i => new DayScheduleForNotification(i)).ToList();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void EnableAllNotifications(int delay = 0)
        {
            if (TodaySchedule.Count == 0) return;

            DayScheduleForNotification prev = TodaySchedule[0];
            foreach (var item in TodaySchedule)
            {
                // исключения:
                // никаких напоминаний заранее, если между парами нет окон
                var first = TimeOnly.Parse(prev.DaySchedule.FinishedAt);
                var second = TimeOnly.Parse(item.DaySchedule.StartedAt);
                bool hasWindow = delay > 0 && (second - first).TotalMinutes > 30; // 30 на запас
                // никаких напоминаний, если следующая пара - тот же предмет
                bool noEqualSubjects = item.DaySchedule.SubjectName != prev.DaySchedule.SubjectName;

                if (item == prev || (delay > 0 && hasWindow) || noEqualSubjects)
                {
                    SetNotification(item, delay);
                }

                prev = item;
            }
        }

        private static bool SetNotification(DayScheduleForNotification item, int delay)
        {
            if (!item.IsNotificationEnabled) return false;

            var time = TimezoneConvertionService.Convert(item.DaySchedule.StartedAt);
            time = time.AddMinutes(delay * -1);
            string timerId = CreateId(item.DaySchedule);
            TaskService.ScheduleTask(timerId, TimeOnly.FromDateTime(time), () => OnTimerElapsed?.Invoke(item.DaySchedule, delay));

            return item.IsNotificationEnabled;
        }

        public static bool EnableNotification(DaySchedule enableForItem, int delay = 0)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == enableForItem.StartedAt);

            if (item is null)
            {
                return false;
            }

            return SetNotification(item, delay);
        }

        public static bool DisableNotification(DaySchedule cancelForItem)
        {
            var item = TodaySchedule.FirstOrDefault(i => i.DaySchedule.StartedAt == cancelForItem.StartedAt);

            if (item is null)
            {
                return false;
            }

            return DisableNotification(item);
        }

        private static bool DisableNotification(DayScheduleForNotification cancelForItem)
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

        private static string CreateId(DaySchedule daySchedule)
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
