using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    public enum NotificationDelayMode
    {
        Delayed,
        WithoutDelay,
        Both
    }
    public class ScheduleNotificationSettings
    {
        public bool Enabled { get; set; } = false;
        public int Delay { get; set; } = 5;
        public bool OnlyFirstSchedule { get; set; } = false;
        public NotificationDelayMode Mode { get; set; } = NotificationDelayMode.Both;
    }
}
