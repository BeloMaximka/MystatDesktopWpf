using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MystatDesktopWpf.Domain
{
    internal class CustomMediaPlayer : MediaPlayer
    {
        public bool HasOwnVolume { get; set; } = false;
    }
    static class SoundCachingPlayer
    {
        static Dictionary<string, CustomMediaPlayer> sounds = new();
        static string workingPath = "./Resources/";

        static double volume = 1;
        static public double Volume
        {
            get => volume;
            set
            {
                volume = value;
                foreach (var item in sounds)
                {
                    if (item.Value.HasOwnVolume == false)
                        item.Value.Volume = volume;
                }
            }
        }
        public static void Play(string name)
        {
            CustomMediaPlayer? player;
            sounds.TryGetValue(name, out player);
            player ??= LoadSound(name);
            player.Stop();
            player.Play();
            
        }
        public static void SetVolume(string name, double volume)
        {
            CustomMediaPlayer? player;
            sounds.TryGetValue(name, out player);
            if (player != null)
            {
                player.Volume = volume;
                player.HasOwnVolume = true;
            }
                
        }
        static CustomMediaPlayer LoadSound(string name)
        {
            CustomMediaPlayer player = new();
            player.Open(new Uri(workingPath + name, UriKind.Relative));
            player.Volume = volume;
            sounds[name] = player;
            return player;
        }
    }
}