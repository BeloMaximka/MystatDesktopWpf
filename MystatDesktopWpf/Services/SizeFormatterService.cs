using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Services
{
    internal static class SizeFormatterService
    {
        static string[] sizeLabelKeys = new string[] { "size-b", "size-kb", "size-mb", "size-gb" };

        public static string Format(long sizeInBytes)
        {
            double cacheSize = sizeInBytes;
            string label = sizeLabelKeys[0];

            int i = 0;
            while (cacheSize >= 1024)
            {
                cacheSize = Math.Round(cacheSize / 1024.0, 2, MidpointRounding.ToZero);
                label = sizeLabelKeys[++i];
            }

            return $"{cacheSize} {App.Current.FindResource(label)}";
        }
    }
}
