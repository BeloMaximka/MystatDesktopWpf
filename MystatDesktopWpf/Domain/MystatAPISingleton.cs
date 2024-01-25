using MystatAPI;
using MystatAPI.Entity;
using MystatDesktopWpf.Services;
using System.Threading.Tasks;

namespace MystatDesktopWpf.Domain
{
    internal static class MystatAPISingleton
    {
        static public MystatAPIClient Client { get; private set; }
        static public ProfileInfo ProfileInfo { get; private set; }
        static async public Task<MystatAuthResponse> LoginAndGetProfileInfo()
        {
            var result = await Client.Login();
            if (result is MystatAuthSuccess responseSuccess)
            {
                MystatAPICachingService.Login = Client.LoginData.Username;
				ProfileInfo = await MystatAPICachingService.GetAndUpdateCachedProfileInfo();
				Client.GroupId = ProfileInfo.CurrentGroupId;
			}
            return result;
        }

        static MystatAPISingleton()
        {
            Client = new MystatAPIClient();
            Client.BypassUploadRestrictions = SettingsService.Settings.Experimental.BypassUploadRestrictions;
        }
    }
}