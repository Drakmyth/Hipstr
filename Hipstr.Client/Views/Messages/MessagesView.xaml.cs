using Hipstr.Client.Utility;
using Hipstr.Core.Models;
using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Messages
{
	public sealed partial class MessagesView : Page
	{
		public MessagesViewModel ViewModel => DataContext as MessagesViewModel;
		private bool _autoScrollMessages;

		public MessagesView()
		{
			InitializeComponent();
			DataContext = new MessagesViewModel();
			_autoScrollMessages = true;
		}

		protected override async void OnNavigatedTo(NavigationEventArgs e)
		{
			ViewModel.Room = (Room)e.Parameter;
			await ViewModel.ReloadMessagesAsync();

			var scrollViewer = MessagesListView.GetFirstDescendantOfType<ScrollViewer>();

			scrollViewer.DirectManipulationStarted += ScrollViewerOnDirectManipulationStarted;
			scrollViewer.DirectManipulationCompleted += ScrollViewerOnDirectManipulationCompleted;
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