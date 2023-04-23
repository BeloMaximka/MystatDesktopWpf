using MystatDesktopWpf.Domain;
using System.Windows.Controls;

namespace MystatDesktopWpf.UserControls
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : UserControl, IRefreshable
    {
        public Settings()
        {
            InitializeComponent();
        }

		public void Refresh()
		{
            CacheSettings.UpdateCacheSize();
		}
	}
}
