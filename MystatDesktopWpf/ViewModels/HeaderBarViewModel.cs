using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MystatDesktopWpf.ViewModels
{
    internal class HeaderBarViewModel : ViewModelBase
    {
        public HeaderBarViewModel()
        {
            AutoRefresh();
        }

        private string name = "";
        public string Name { get => name; private set => SetProperty(ref name, value); }

        private string group = "";
        public string Group { get => group; private set => SetProperty(ref group, value); }

        private int diamonds;
        public int Diamonds { get => diamonds; private set => SetProperty(ref diamonds, value); }

        private int coins;
        public int Coins { get => coins; private set => SetProperty(ref coins, value); }

        private int badges;
        public int Badges { get => badges; private set => SetProperty(ref badges, value); }

        public int Points { get => Diamonds + Coins; }

        private void AutoRefresh()
        {
            Refresh();
            TaskService.CancelTask("header-bar-refresh");
            TaskService.ScheduleTask("header-bar-refresh", TimeSpan.FromHours(8), () =>
            {
                AutoRefresh();
            });
        }

        private bool loading = false;
        public async void Refresh()
        {
            if (loading) return;
            loading = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    ProfileInfo info = await MystatAPICachingService.GetAndUpdateCachedProfileInfo();
                    Name = info.FullName;
                    Group = info.GroupName;
                    Badges = info.AchievesCount;

                    Diamonds = info.Points.First((s) => s.PointType == GamingPointTypes.Gems).PointsCount;
                    Coins = info.Points.First((s) => s.PointType == GamingPointTypes.Coins).PointsCount;
                    OnPropertyChanged(nameof(Points));
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(delay.ProvideValueMilliseconds());
                    continue;
                }
            }
            loading = false;
        }
    }
}
