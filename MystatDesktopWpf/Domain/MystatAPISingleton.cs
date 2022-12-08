using MystatAPI;

namespace MystatDesktopWpf.Domain
{
    internal static class MystatAPISingleton
    {
        static public MystatAPIClient Client { get; private set; }
        static MystatAPISingleton()
        {
            Client = new MystatAPIClient();
        }
    }
}