using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class AddTeamView : Page
	{
		public AddTeamViewModel ViewModel => DataContext as AddTeamViewModel;

		public AddTeamView()
		{
			InitializeComponent();
			DataContext = new AddTeamViewModel();
		}
	}
}
