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
		private bool _firstScroll;

		public static readonly DependencyProperty StartAtBottomProperty = DependencyProperty.Register(
			nameof(StartAtBottom),
			typeof(bool),
			typeof(StickyScrollBehavior),
			new PropertyMetadata(false));

		public bool StartAtBottom
		{
			get { return (bool)GetValue(StartAtBottomProperty); }
			set { SetValue(StartAtBottomProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			_autoScroll = true;
			_firstScroll = StartAtBottom;
			AssociatedObject.Loaded += AssociatedObject_OnLoaded;
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			_scrollViewer.ViewChanged -= ScrollViewerOnViewChanged;
			AssociatedObject.LayoutUpdated -= AssociatedObject_OnLayoutUpdated;
		}

		private void AssociatedObject_OnLoaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Loaded -= AssociatedObject_OnLoaded;

			_scrollViewer = AssociatedObject.GetFirstDescendantOfType<ScrollViewer>();

			_scrollViewer.ViewChanged += ScrollViewerOnViewChanged;
			AssociatedObject.LayoutUpdated += AssociatedObject_OnLayoutUpdated;
		}

		private void ScrollViewerOnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if (e.IsIntermediate)
			{
				_autoScroll = false;
			}
			else if (Math.Abs(_scrollViewer.VerticalOffset - _scrollViewer.ScrollableHeight) < 0.01f)
			{
				_autoScroll = true;
			}
		}

		private void AssociatedObject_OnLayoutUpdated(object sender, object e)
		{
			if (!_autoScroll) return;

			_scrollViewer.ChangeView(null, _scrollViewer.ScrollableHeight, null, _firstScroll);
			_scrollViewer.UpdateLayout();

			_firstScroll = _firstScroll && _scrollViewer.ScrollableHeight < 0.01f;
		}
	}
}