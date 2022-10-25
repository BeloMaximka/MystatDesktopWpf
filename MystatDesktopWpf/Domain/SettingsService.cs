using MystatAPI.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Markup;

namespace MystatDesktopWpf.Domain
{
    internal static class SettingsService
    {
        const string settingsFilePath = "./settings.bin";
        private static int EncryptedLength = 0;
        public static Settings Settings { get; private set; }

        public static event Action OnSettingsChange;

        static SettingsService()
        {
            Settings = Load() ?? new();
            OnSettingsChange += () => Save();
        }

        public static Settings? Load()
        {
            try
            {
                RemoveEncryption();
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
                AddEncryption();
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

        public static bool SetPropertyValue(string property, object value)
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

        private static string DecryptStringFromBytes(byte[] encrypted, byte[] key, byte[] IV)
        {
            string plaintext = null;

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = key;
                rijAlg.IV = IV;

                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encrypted))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }

            return plaintext;

        }

        private static byte[] EncryptStringToBytes(string original, byte[] key, byte[] IV)
        {
            byte[] encrypted;

            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = key;
                rijAlg.IV = IV;

                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(original);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            return encrypted;
        }
        public static void AddEncryption()
        {
            var allText = File.ReadAllText(settingsFilePath);
            using (Rijndael myRijndael = Rijndael.Create())
            {
                byte[] encrypted = EncryptStringToBytes(allText, myRijndael.Key, myRijndael.IV);
                using var writer = new BinaryWriter(File.OpenWrite(settingsFilePath));
                writer.Write(encrypted);
                writer.Close();
                EncryptedLength = encrypted.Length;
            }
        }

        public static void RemoveEncryption()
        {
            using (Rijndael myRijndael = Rijndael.Create())
            {
                var reader = new BinaryReader(File.OpenRead(settingsFilePath));
                var encrypted = reader.ReadBytes(EncryptedLength);
                var roundtrip = DecryptStringFromBytes(encrypted, myRijndael.Key, myRijndael.IV);
            }
        }
    }
    internal class Settings
    {
        public UserLoginData? LoginData { get; set; }
        public bool TimerEnabled { get; set; } = true;
    }
}
