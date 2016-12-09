using Hipstr.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Messages
{
	public sealed partial class MessagesView : Page
	{
		public MessagesViewModel ViewModel => DataContext as MessagesViewModel;

		public MessagesView()
		{
			InitializeComponent();
			DataContext = new MessagesViewModel();
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Room = (Room)e.Parameter;
			await ViewModel.ReloadMessagesAsync();
		}
	}
}
