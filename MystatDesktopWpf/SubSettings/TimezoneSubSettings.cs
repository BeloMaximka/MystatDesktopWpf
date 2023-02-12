using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MystatDesktopWpf.SubSettings
{
    internal class TimezoneSubSettings : ISettingsProperty
    {
        public event Action? OnPropertyChanged;

        TimeZoneInfo from = TimeZoneInfo.Local;
        TimeZoneInfo to = TimeZoneInfo.Local;

        [JsonConverter(typeof(TimeZoneConverter))]
        public TimeZoneInfo From
        {
            get => from;
            set
            {
                from = value;
                PropertyChanged();
            }
        }
        
        [JsonConverter(typeof(TimeZoneConverter))]
        public TimeZoneInfo To
        {
            get => to;
            set
            {
                to = value;
                PropertyChanged();
            }
        }

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }

    [JsonConverter(typeof(TimeZoneConverter))]
    public class TimeZoneConverter : JsonConverter<TimeZoneInfo>
    {
        public override TimeZoneInfo? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? serializedTimezone = reader.GetString();
            return serializedTimezone is null ? null : TimeZoneInfo.FromSerializedString(serializedTimezone);
        }

        public override void Write(Utf8JsonWriter writer, TimeZoneInfo value, JsonSerializerOptions options)
        {
            writer.WriteRawValue($"\"{value.ToSerializedString()}\"");
        }
    }
}
