using Hipstr.Core.Models;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Messages
{
	public sealed partial class MessagesView : Page
	{
		public MessagesView()
		{
			InitializeComponent();
		}

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			((MessagesViewModel)DataContext).Room = (Room)e.Parameter;
		}
	}
}
