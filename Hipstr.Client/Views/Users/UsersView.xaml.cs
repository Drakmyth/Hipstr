using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Users
{
	public sealed partial class UsersView : Page
	{
		public UsersViewModel ViewModel => DataContext as UsersViewModel;

		public UsersView()
		{
			InitializeComponent();
			DataContext = new UsersViewModel();
		}
	}
}
