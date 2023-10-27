using MystatAPI;
using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using MystatDesktopWpf.Services;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

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

        private Group group;
        //public Group Group { get => group; set => SetProperty(ref group, value); }
        public Group Group { get => group; set
            {
                int? currentGroupId = group?.Id;
                int newGroupId = value.Id;
                SetProperty(ref group, value);
                if(currentGroupId is not null && currentGroupId != newGroupId)
                {
                    MystatAPISingleton.Client.ChangeCurrentGroup(newGroupId)
                        .ContinueWith(success =>
                        {
                            if(success.Result)
                            {
                                // refresh all
                                App.NotifyGroupChanged();
                                Refresh(true);
                            }
                            else
                            {
                                // show error
                            }
                        });
                }
            }
        }

        private int diamonds;
        public int Diamonds { get => diamonds; private set => SetProperty(ref diamonds, value); }

        private int coins;
        public int Coins { get => coins; private set => SetProperty(ref coins, value); }

        private int badges;
        public int Badges { get => badges; private set => SetProperty(ref badges, value); }
        
        private bool isMultipleGroups;
        public bool IsMultipleGroups { get => isMultipleGroups; private set => SetProperty(ref isMultipleGroups, value); }

        private Group[] groupsList;
        public Group[] GroupsList { get => groupsList; private set => SetProperty(ref groupsList, value); }

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
        private bool updatedOnStartup = false;
        public async void Refresh(bool uncached = false)
        {
            if (loading) return;
            loading = true;
            RetryDelayProvider delay = new();
            while (true)
            {
                try
                {
                    ProfileInfo info = await MystatAPICachingService.GetAndUpdateCachedProfileInfo(uncached);
                    Name = info.FullName;
                    Group = info.Groups.First(g => g.Id == info.CurrentGroupId);
                    IsMultipleGroups = info.Groups.Length > 1;
                    GroupsList = info.Groups;
                    Badges = info.AchievesCount;

                    Diamonds = info.Points.First((s) => s.PointType == GamingPointTypes.Gems).PointsCount;
                    Coins = info.Points.First((s) => s.PointType == GamingPointTypes.Coins).PointsCount;
                    OnPropertyChanged(nameof(Points));
                    if(!updatedOnStartup)
                    {
                        await MystatAPICachingService.GetAndUpdateCachedProfileInfo(uncached: true);
                        updatedOnStartup = true;
                        loading = false;
                        Refresh();
                    }
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
