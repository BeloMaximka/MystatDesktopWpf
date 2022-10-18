using MystatAPI;
using MystatAPI.Entity;

namespace MystatDesktopWpf.Domain
{
    internal class MystatAPISingleton
    {
        static public MystatAPIClient mystatAPIClient { get; private set; }
        static MystatAPISingleton()
        {
            mystatAPIClient = new MystatAPIClient(new UserLoginData());
        }
    }
}