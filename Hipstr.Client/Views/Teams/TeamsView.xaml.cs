using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class TeamsView : Page
	{
		public TeamsViewModel ViewModel => DataContext as TeamsViewModel;

		public TeamsView()
		{
			InitializeComponent();
			DataContext = new TeamsViewModel();
		}
	}
}
