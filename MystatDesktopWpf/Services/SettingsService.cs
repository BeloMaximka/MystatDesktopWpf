using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using MystatDesktopWpf.Domain;

namespace MystatDesktopWpf.Services
{
    internal static class SettingsService
    {
        const string settingsFilePath = "./settings.bin";
        public static Settings Settings { get; private set; }

        public static event Action OnSettingsChange;

        static SettingsService()
        {
            Settings = Load() ?? new();

            CancellationTokenSource? cancelTokenSource = null;
            var saveSettingsDelay = TimeSpan.FromSeconds(3);
            var saveDebounced = () =>
            {
                cancelTokenSource?.Cancel();
                cancelTokenSource = new CancellationTokenSource();

                Task.Delay(saveSettingsDelay, cancelTokenSource.Token)
                    .ContinueWith(t =>
                    {
                        if (t.IsCompletedSuccessfully)
                        {
                            Save();
                        }
                    }, TaskScheduler.Default);
            };

            OnSettingsChange += saveDebounced;
            Settings.ScheduleNotification.OnPropertyChanged += saveDebounced;
            Settings.Theme.OnPropertyChanged += saveDebounced;
        }

        public static Settings? Load()
        {
            try
            {
                string? content = File.ReadAllText(settingsFilePath);
                var settings = JsonSerializer.Deserialize<Settings>(content);

                if (settings?.LoginData is not null)
                {
                    settings.LoginData.Password = TransformPassword(settings.LoginData.Password);
                }

                return settings;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static bool Save(Settings newSettings)
        {
            Settings = newSettings;
            return Save();
        }

        public static bool Save()
        {
            try
            {
                File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(Settings));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool SetLoginData(UserLoginData loginData)
        {
            var copy = new UserLoginData(loginData.Username, TransformPassword(loginData.Password));
            return SetPropertyValue(nameof(Settings.LoginData), copy);
        }

        public static bool RemoveUserData()
        {
            return SetPropertyValue(nameof(Settings.LoginData), null);
        }

        public static bool SetPropertyValue(string property, object? value)
        {
            Type type = Settings.GetType();
            PropertyInfo? prop = type.GetProperty(property);

            if (prop is null) return false;

            prop.SetValue(Settings, value);
            OnSettingsChange();
            return true;
        }

        private static string TransformPassword(string password)
        {
            // TODO: encrypt/decrypt password
            return password;
        }
    }

    internal interface ISettingsProperty
    {
        event Action? OnPropertyChanged;
        void PropertyChanged();
    }

    internal class Settings
    {
        public UserLoginData? LoginData { get; set; }
        public ScheduleNotificationSubSettings ScheduleNotification { get; set; } = new();
        public ThemeSubSettings Theme { get; set; } = new();
    }
}
