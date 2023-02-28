namespace MystatDesktopWpf.Domain
{
    internal class RetryDelayProvider
    {
        private static readonly int[] delaysSeconds = { 1, 1, 2, 4, 8, 16, 32 };
        private int index = 0;

        private int Index
        {
            get { return index; }
            set
            {
                if (value + 1 < delaysSeconds.Length) index = value;
            }
        }
        public int ProvideValueMilliseconds() => delaysSeconds[++Index] * 1000;

    }
}
