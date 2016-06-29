using System.Diagnostics;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Hipstr.Core.Models;

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
			Debug.WriteLine(e.Parameter?.GetType());
		}
	}
}
