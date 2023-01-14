using MystatAPI;
using MystatAPI.Entity;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal static class MystatAPISingleton
    {
        static public MystatAPIClient Client { get; private set; }
        static public ProfileInfo? Profile { get; private set; }
        static async public Task<MystatAuthResponse> LoginAndGetProfileInfo()
        {
            var result = await Client.Login();
            if (result is MystatAuthSuccess responseSuccess)
                Profile = await Client.GetProfileInfo();
            return result;
        }
        static MystatAPISingleton()
        {
            Client = new MystatAPIClient();
        }
    }
}