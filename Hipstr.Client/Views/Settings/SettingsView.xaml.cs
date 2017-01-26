using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Settings
{
	public sealed partial class SettingsView : Page
	{
		public SettingsViewModel ViewModel => (SettingsViewModel)DataContext;

		public SettingsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<SettingsViewModel>();
		}
	}
}