using Hipstr.Core.Messaging;
using System;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Messages
{
	public sealed partial class MessagesView : Page
	{
		public MessagesViewModel ViewModel => (MessagesViewModel)DataContext;
		private bool _pollForMessages;

		public MessagesView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<MessagesViewModel>();
			_pollForMessages = true;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.MessageSource = (IMessageSource)e.Parameter;
			await ViewModel.ReloadMessagesAsync();

			while (_pollForMessages)
			{
				await Task.Delay(TimeSpan.FromSeconds(30));
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await ViewModel.CheckForNewMessages(); });
			}
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			_pollForMessages = false;
		}
	}
}