using MystatAPI;
using MystatAPI.Entity;

namespace MystatDesktopWpf.Domain
{
    static class MystatAPISingleton
    {
        static public MystatAPIClient mystatAPIClient { get; private set; }
        static MystatAPISingleton()
        {
            mystatAPIClient = new MystatAPIClient();
        }
    }
}