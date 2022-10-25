using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace MystatDesktopWpf.Domain
{
    internal static class TaskService
    {
        static Dictionary<string, DispatcherTimer> timers;

        static TaskService()
        {
            timers = new();
        }

        public static bool ScheduleTask(string id, TimeOnly time, Action callback)
        {
            var currentTimeSpan = DateTime.Now.TimeOfDay;
            var targetTimeSpan = time.ToTimeSpan();

            if (currentTimeSpan > targetTimeSpan)
            {
                return false;
            }

            TimeSpan duration = targetTimeSpan - currentTimeSpan;
            var timer = new DispatcherTimer();
            timer.Interval = duration;
            timer.Tick += (_, _) => OnTimerEnd(id, timer, callback);
            timer.Start();

            timers.Add(id, timer);
            return true;
        }

        public static bool CancelTask(string id)
        {
            var timer = timers.FirstOrDefault(kv => kv.Key == id).Value;

            if (timer is null) return false;

            StopTimer(id, timer);
            return true;
        }

        static void OnTimerEnd(string id, DispatcherTimer timer, Action callback)
        {
            StopTimer(id, timer);
            callback();
        }

        static void StopTimer(string id, DispatcherTimer timer)
        {
            timer.Stop();
            timers.Remove(id);
        }
    }
}
