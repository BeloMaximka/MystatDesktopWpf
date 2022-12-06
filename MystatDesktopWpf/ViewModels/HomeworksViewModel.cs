using MystatAPI.Entity;
using MystatDesktopWpf.Domain;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MystatDesktopWpf.ViewModels
{
    internal class HomeworksViewModel : ViewModelBase
    {
        public event Action HomeworkLoaded;
        public bool Loading { get; private set; } = false;

        public async void LoadHomeworks()
        {
            while (true)
            {
                try
                {
                    Loading = true;
                    var result = await MystatAPISingleton.mystatAPIClient.GetHomework(1, HomeworkStatus.Active);
                    Active = new(result);
                    result = await MystatAPISingleton.mystatAPIClient.GetHomework(1, HomeworkStatus.Overdue);
                    Overdue = new(result);
                    result = await MystatAPISingleton.mystatAPIClient.GetHomework(1, HomeworkStatus.Uploaded);
                    Uploaded = new(result);
                    result = await MystatAPISingleton.mystatAPIClient.GetHomework(1, HomeworkStatus.Checked);
                    Checked = new(result);
                    result = await MystatAPISingleton.mystatAPIClient.GetHomework(1, HomeworkStatus.Deleted);
                    Deleted = new(result);
                    Loading = false;
                    HomeworkLoaded?.Invoke();
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(2000);
                    continue;
                }
            }
        }
        
        ObservableCollection<Homework> active = new();
        public ObservableCollection<Homework> Active
        {
            get => active;
            set
            {
                active = value;
                OnPropertyChanged();
            }
        }
        ObservableCollection<Homework> overdue = new();
        public ObservableCollection<Homework> Overdue
        {
            get => overdue;
            set
            {
                overdue = value;
                OnPropertyChanged();
            }
        }
        ObservableCollection<Homework> uploaded = new();
        public ObservableCollection<Homework> Uploaded
        {
            get => uploaded;
            set
            {
                uploaded = value;
                OnPropertyChanged();
            }
        }
        ObservableCollection<Homework> _checked = new();
        public ObservableCollection<Homework> Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                OnPropertyChanged();
            }
        }
        ObservableCollection<Homework> deleted = new();
        public ObservableCollection<Homework> Deleted
        {
            get => deleted;
            set
            {
                deleted = value;
                OnPropertyChanged();
            }
        }
    }
}
