using Hipstr.Client.Utility;
using Microsoft.Xaml.Interactivity;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Behaviors
{
	public class StickyScrollBehavior : Behavior<FrameworkElement>
	{
		private bool _autoScroll;
		private ScrollViewer _scrollViewer;

		protected override void OnAttached()
		{
			base.OnAttached();
			_autoScroll = true;
			AssociatedObject.Loaded += OnAssociatedObjectLoaded;
		}

		protected override void OnDetaching()
		{
			_scrollViewer.DirectManipulationStarted -= ScrollViewerOnDirectManipulationStarted;
			_scrollViewer.DirectManipulationCompleted -= ScrollViewerOnDirectManipulationCompleted;
		}

		private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;

			_scrollViewer = AssociatedObject.GetFirstDescendantOfType<ScrollViewer>();

			_scrollViewer.DirectManipulationStarted += ScrollViewerOnDirectManipulationStarted;
			_scrollViewer.DirectManipulationCompleted += ScrollViewerOnDirectManipulationCompleted;
			AssociatedObject.LayoutUpdated += AssociatedObject_OnLayoutUpdated;
		}

		private void AssociatedObject_OnLayoutUpdated(object sender, object e)
		{
			if (!_autoScroll) return;

			_scrollViewer.ChangeView(null, _scrollViewer.ScrollableHeight, null, true);
			_scrollViewer.UpdateLayout();
		}

		private void ScrollViewerOnDirectManipulationCompleted(object sender, object o)
		{
			if (Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) < 0.01f)
			{
				_autoScroll = true;
			}
		}

		private void ScrollViewerOnDirectManipulationStarted(object sender, object o)
		{
			_autoScroll = false;
		}
	}
}