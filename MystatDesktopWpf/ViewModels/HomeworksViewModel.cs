using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.ComponentModel;

namespace MystatDesktopWpf.ViewModels
{
    internal class HomeworksViewModel : ViewModelBase, INotificationCount
    {
        public bool Loading { get; private set; } = false;
        public bool LoadedOnce { get; private set; } = false;

        public HomeworksViewModel()
        {
            Homework[HomeworkStatus.Active].PropertyChanged += HomeworksViewModel_PropertyChanged;
            Homework[HomeworkStatus.Overdue].PropertyChanged += HomeworksViewModel_PropertyChanged;
            Homework[HomeworkStatus.Deleted].PropertyChanged += HomeworksViewModel_PropertyChanged;
            LoadHomework();
        }

        Task<bool>? homeworkTask = null;
        public async Task<bool> LoadHomework()
        {
            if (homeworkTask != null) return await homeworkTask;

            homeworkTask = LoadHomework_Body();
            bool result = await homeworkTask;
            homeworkTask = null;
            return result;
        }

        private async Task<bool> LoadHomework_Body()
        {
            try
            {
                var homeworkCount = await MystatAPISingleton.Client.GetHomeworkCount();
                foreach (var item in homeworkCount)
                {
                    if (Homework.TryGetValue(item.Status, out HomeworkCollection collection))
                    {
                        collection.MaxCount = item.Count;
                        collection.Page = 1;
                    }
                }

                foreach (var item in Homework)
                {
                    var HomeworkResult = await MystatAPISingleton.Client.GetHomework(1, item.Key);
                    Homework[item.Key].Items = new(HomeworkResult);
                }

                LoadedOnce = true;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void AddHomework(Homework homework)
        {
            var collection = Homework[homework.Status];
            collection.Items.Add(homework);
            collection.MaxCount++;
        }

        public void DeleteHomework(Homework homework)
        {
            var collection = Homework[homework.Status];
            collection.Items.Remove(homework);
            collection.MaxCount--;
        }

        public void MoveHomework(Homework homework, HomeworkStatus destination)
        {
            DeleteHomework(homework);
            homework.Status = destination;
            AddHomework(homework);
        }

        private void HomeworksViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "MaxCount")
                OnPropertyChanged(nameof(MenuItemNotifications));
        }

        public Dictionary<HomeworkStatus, HomeworkCollection> Homework { get; set; } = new()
        {
            { HomeworkStatus.Overdue, new() },
            { HomeworkStatus.Checked, new() },
            { HomeworkStatus.Uploaded, new() },
            { HomeworkStatus.Active, new() },
            { HomeworkStatus.Deleted, new() }
        };

        public int MenuItemNotifications
        {
            get
            {
                return Homework[HomeworkStatus.Active].MaxCount +
                    Homework[HomeworkStatus.Overdue].MaxCount +
                    Homework[HomeworkStatus.Deleted].MaxCount;
            }
        }
    }

    public class HomeworkCollection : ViewModelBase
    {
        private ObservableCollection<Homework> items = new();
        public ObservableCollection<Homework> Items
        {
            get => items;
            set
            {
                SetProperty(ref items, value);
                OnPropertyChanged(nameof(NoPages));
            }
        }

        public bool NoPages { get => items.Count < maxCount; }

        private int maxCount = new();
        public int MaxCount { get => maxCount; set => SetProperty(ref maxCount, value); }

        public int Page { get; set; } = 1;

        public async Task<bool> LoadNextPage()
        {
            if (Items.Count == 0 || Items.Count == MaxCount) return false;

            try
            {
                Homework[] result = await MystatAPISingleton.Client.GetHomework(++Page, items[0].Status);
                foreach (var item in result)
                    Items.Add(item);
                OnPropertyChanged(nameof(NoPages));
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}