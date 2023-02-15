using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace MystatDesktopWpf.Services
{
    internal static class TaskService
    {
        private static readonly Dictionary<string, DispatcherTimer> timers;

        public static Dictionary<string, DispatcherTimer>.KeyCollection TimersIds
        {
            get => timers.Keys;
        }

        static TaskService()
        {
            timers = new();
        }

        public static bool ScheduleTask(string id, TimeOnly time, Action callback)
        {
            return ScheduleTask(id, DateTime.Now, time, callback);
        }

        public static bool ScheduleTask(string id, TimeSpan delay, Action callback)
        {
            if (timers.ContainsKey(id))
            {
                return false;
            }

            AddTimer(id, delay, callback);
            return true;
        }

        public static bool ScheduleTask(string id, DateTime day, TimeOnly time, Action callback)
        {
            var now = DateTime.Now;
            day = day.Date + time.ToTimeSpan();

            if (day < now)
            {
                return false;
            }

            TimeSpan duration = (day - now).Duration();
            return ScheduleTask(id, duration, callback);
        }

        private static void AddTimer(string id, TimeSpan duration, Action callback)
        {
            DispatcherTimer timer = new() { Interval = duration };
            timer.Tick += (_, _) => OnTimerEnd(id, timer, callback);
            timer.Start();

            timers.Add(id, timer);
        }

        public static bool CancelTask(string id)
        {
            if (!timers.ContainsKey(id)) return false;

            var timer = timers[id];

            StopTimer(id, timer);
            return true;
        }

        private static void OnTimerEnd(string id, DispatcherTimer timer, Action callback)
        {
            StopTimer(id, timer);
            callback();
        }

        private static void StopTimer(string id, DispatcherTimer timer)
        {
            timer.Stop();
            timers.Remove(id);
        }
    }
}
