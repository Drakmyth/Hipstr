using Hipstr.Client.Utility;
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
		private bool _autoScrollMessages;

		public MessagesView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<MessagesViewModel>();
			_autoScrollMessages = true;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.MessageSource = (IMessageSource)e.Parameter;
			await ViewModel.ReloadMessagesAsync();

			var scrollViewer = MessagesListView.GetFirstDescendantOfType<ScrollViewer>();

			scrollViewer.DirectManipulationStarted += ScrollViewerOnDirectManipulationStarted;
			scrollViewer.DirectManipulationCompleted += ScrollViewerOnDirectManipulationCompleted;

			while (true)
			{
				await Task.Delay(TimeSpan.FromSeconds(30));
				await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => { await ViewModel.CheckForNewMessages(); });
			}
		}

		private void MessagesListView_OnLayoutUpdated(object sender, object e)
		{
			if (!_autoScrollMessages) return;

			var scrollViewer = MessagesListView.GetFirstDescendantOfType<ScrollViewer>();
			scrollViewer.ChangeView(null, scrollViewer.ScrollableHeight, null, true);
			scrollViewer.UpdateLayout();
		}

		private void ScrollViewerOnDirectManipulationCompleted(object sender, object o)
		{
			var scrollViewer = MessagesListView.GetFirstDescendantOfType<ScrollViewer>();
			if (Math.Abs(scrollViewer.VerticalOffset - scrollViewer.ScrollableHeight) < 0.01f)
			{
				_autoScrollMessages = true;
			}
		}

		private void ScrollViewerOnDirectManipulationStarted(object sender, object o)
		{
			_autoScrollMessages = false;
		}
	}
}