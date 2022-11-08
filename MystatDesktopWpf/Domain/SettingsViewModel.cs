using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MystatDesktopWpf.Services;

namespace MystatDesktopWpf.Domain
{
    public class SettingsViewModel : ViewModelBase
    {
        ScheduleNotificationSettings schedule;
        public SettingsViewModel()
        {
            schedule = SettingsService.Settings.ScheduleNotification;
        }

        public double NotificationVolume
        {
            set
            {
                OnPropertyChanged("NotificationVolume");
                schedule.Volume = value;
                SoundCachingPlayer.Volume = schedule.Volume;
                SettingsService.Settings.ScheduleNotification.Volume = value;
            }
            get => schedule.Volume;
        }

        public NotificationDelayMode DelayMode
        {
            set
            {
                OnPropertyChanged("DelayMode");
                schedule.Mode = value;
                SettingsService.Settings.ScheduleNotification.Mode = value;
                ScheduleNotificationService.Configure(schedule.Delay, schedule.Mode);
            }
            get => schedule.Mode;
        }

        public bool IsNotOnlyFirstSchedule
        {
            set => OnlyFirstSchedule = !value;
            get => !OnlyFirstSchedule;
        }
        public bool OnlyFirstSchedule
        {
            set
            {
                OnPropertyChanged("OnlyFirstSchedule");
                schedule.OnlyFirstSchedule = value;
                SettingsService.Settings.ScheduleNotification.OnlyFirstSchedule = value;
                ScheduleNotificationService.OnlyFirstSchedule = value;
                ScheduleNotificationService.Configure(schedule.Delay, schedule.Mode);
            }
            get => SettingsService.Settings.ScheduleNotification.OnlyFirstSchedule;
        }
        public int NotificationDelay
        {
            set
            {
                OnPropertyChanged("NotificationDelay");
                schedule.Delay = value;
                SettingsService.Settings.ScheduleNotification.Delay = value;
                ScheduleNotificationService.Configure(schedule.Delay, schedule.Mode);
            }
            get => SettingsService.Settings.ScheduleNotification.Delay;
        }
        public bool NotificationEnabled
        {
            set
            {
                OnPropertyChanged("NotificationEnabled");
                schedule.Enabled = value;
                SettingsService.Settings.ScheduleNotification.Enabled = value;

                if (value)
                    ScheduleNotificationService.Configure(schedule.Delay, schedule.Mode);
                    
                else
                    ScheduleNotificationService.DisableAllNotifications();
            }
            get => SettingsService.Settings.ScheduleNotification.Enabled;
        }
    }
}
