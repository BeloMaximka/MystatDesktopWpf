﻿using MystatAPI.Entity;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Services
{
    internal static class SettingsService
    {
        private static readonly string settingsFilePath;
        public static Settings Settings { get; private set; }

        public static event Action OnSettingsChange;

        static SettingsService()
        {
            Directory.CreateDirectory(Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat"));
            settingsFilePath = Environment.ExpandEnvironmentVariables(@"%appdata%\Mystat\settings.bin");

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
            Settings.ScheduleNotification.OnPropertyChanged += OnSettingsChange;
            Settings.Theme.OnPropertyChanged += OnSettingsChange;
            Settings.Tray.OnPropertyChanged += OnSettingsChange;
        }

        public static Settings? Load()
        {
            try
            {
                string? content = File.ReadAllText(settingsFilePath);
                var settings = JsonSerializer.Deserialize<Settings>(content);

                if (settings?.LoginData is not null)
                {
                    settings.LoginData.Password = DecpyptPassword(settings.LoginData.Password);
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
            var copy = new UserLoginData(loginData.Username, EncpyptPassword(loginData.Password));
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

        private static string EncpyptPassword(string password)
        {
            StringBuilder newPass = new();

            foreach (var ch in password)
            {
                newPass.Append((char)~ch);
            }

            return newPass.ToString();
        }

        private static string DecpyptPassword(string password)
        {
            StringBuilder newPass = new();

            foreach (var en in password)
            {
                newPass.Append((char)~en);
            }

            return newPass.ToString();
        }
    }

    internal interface ISettingsProperty
    {
        event Action? OnPropertyChanged;
        void PropertyChanged();
    }

    internal class Settings
    {
        public Settings()
        {
            /* Если у нас нет языка, который стоит у пользователя (например, японский),
               то мы ставим анлийский*/
            if (App.Languages.Contains(CultureInfo.InstalledUICulture))
                Language = CultureInfo.InstalledUICulture.Name;
            else
                Language = new CultureInfo("en-US").Name;
        }
        public UserLoginData? LoginData { get; set; }
        public ScheduleNotificationSubSettings ScheduleNotification { get; set; } = new();
        public ThemeSubSettings Theme { get; set; } = new();
        public TraySubSettings Tray { get; set; } = new();
        public string Language { get; set; }
    }
}
