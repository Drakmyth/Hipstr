using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

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

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			await ViewModel.UpdateUserListAsync();
		}
	}
}