using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal class RetryDelayProvider
    {
        readonly static int[] delaysSeconds = { 1, 1, 2, 2, 4, 8, 16 };
        int index = 0;
        int Index
        {
            get { return index; }
            set
            {
                if (value + 1 >= delaysSeconds.Length) index = value;
            }
        }
        public int ProvideValueMilliseconds() => delaysSeconds[++Index];

    }
}
