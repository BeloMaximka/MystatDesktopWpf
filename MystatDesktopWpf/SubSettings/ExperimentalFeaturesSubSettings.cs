using MystatDesktopWpf.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MystatDesktopWpf.SubSettings
{
    public class ExperimentalFeaturesSubSettings : ISettingsProperty
    {
        public event Action? OnPropertyChanged;

        private bool bypassUploadRestrictions = false;

        public bool BypassUploadRestrictions
        {
            get => bypassUploadRestrictions;
            set
            {
                bypassUploadRestrictions = value;
                PropertyChanged();
            }
        }

        public void PropertyChanged()
        {
            OnPropertyChanged?.Invoke();
        }
    }
}
