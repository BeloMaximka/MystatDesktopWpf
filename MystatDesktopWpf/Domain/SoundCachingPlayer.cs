using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MystatDesktopWpf.Domain
{
    /* TODO
     * - группировка звуков
     * - изменение звука для групп
     * - решение проблемы с щелкающим звуком при вызовы метода Load()
     */
    static class SoundCachingPlayer
    {
        static Dictionary<string, MediaPlayer> sounds = new();
        static string workingPath = "./Resources/";

        static double volume = 1;
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
            MediaPlayer? player;
            sounds.TryGetValue(name, out player);
            player ??= LoadSound(name);
            player.Stop();
            player.Play();
            
        }
        static MediaPlayer LoadSound(string name)
        {
            MediaPlayer player = new();
            player.Open(new Uri(workingPath + name, UriKind.Relative));
            player.Volume = volume;
            sounds[name] = player;
            return player;
        }
    }
}