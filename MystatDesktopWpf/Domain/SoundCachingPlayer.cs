using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace MystatDesktopWpf.Domain
{
    /* TODO
     * - группировка звуков
     * - изменение звука для групп
     * - решение проблемы с щелкающим звуком при вызовы метода Load()
     */
    internal static class SoundCachingPlayer
    {
        private static readonly Dictionary<string, MediaPlayer> sounds = new();
        private static readonly string workingPath = "./Resources/";
        private static double volume = 1;
        static public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                foreach (var item in sounds)
                    item.Value.Volume = volume;
            }
        }
        public static void Play(string name)
        {
            sounds.TryGetValue(name, out MediaPlayer? player);
            player ??= LoadSound(name);
            player.Stop();
            player.Play();

        }

        private static MediaPlayer LoadSound(string name)
        {
            MediaPlayer player = new();
            player.Open(new Uri(workingPath + name, UriKind.Relative));
            player.Volume = volume;
            sounds[name] = player;
            return player;
        }
    }
}