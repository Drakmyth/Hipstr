using System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Hipstr.Client.Controls
{
	public sealed partial class SplitViewSplitter : UserControl
	{
		private double _dragOffset;
		private bool _resetCursor;

		public UIElement LeftPane
		{
			get { return (UIElement)GetValue(LeftPaneProperty); }
			set { SetValue(LeftPaneProperty, value); }
		}

		public static readonly DependencyProperty LeftPaneProperty =
			DependencyProperty.Register(nameof(LeftPane), typeof(UIElement), typeof(SplitViewSplitter), new PropertyMetadata(null));

		public UIElement RightPane
		{
			get { return (UIElement)GetValue(RightPaneProperty); }
			set { SetValue(RightPaneProperty, value); }
		}

		public static readonly DependencyProperty RightPaneProperty =
			DependencyProperty.Register(nameof(RightPane), typeof(UIElement), typeof(SplitViewSplitter), new PropertyMetadata(null));

		public double AnchorPosition
		{
			get { return (double)GetValue(AnchorPositionProperty); }
			set { SetValue(AnchorPositionProperty, value); }
		}

		public static readonly DependencyProperty AnchorPositionProperty =
			DependencyProperty.Register(nameof(AnchorPosition), typeof(double), typeof(SplitViewSplitter), new PropertyMetadata(0.0));

		public double AnchorWidth
		{
			get { return (double)GetValue(AnchorWidthProperty); }
			set { SetValue(AnchorWidthProperty, value); }
		}

		public static readonly DependencyProperty AnchorWidthProperty =
			DependencyProperty.Register(nameof(AnchorWidth), typeof(double), typeof(SplitViewSplitter), new PropertyMetadata(10.0));

		public Brush AnchorBackground
		{
			get { return (Brush)GetValue(AnchorBackgroundProperty); }
			set { SetValue(AnchorBackgroundProperty, value); }
		}

		public static readonly DependencyProperty AnchorBackgroundProperty =
			DependencyProperty.Register(nameof(AnchorBackground), typeof(Brush), typeof(SplitViewSplitter), new PropertyMetadata(Application.Current.Resources["SystemControlBackgroundChromeMediumLowBrush"]));

		public Brush AnchorForeground
		{
			get { return (Brush)GetValue(AnchorForegroundProperty); }
			set { SetValue(AnchorForegroundProperty, value); }
		}

		public static readonly DependencyProperty AnchorForegroundProperty =
			DependencyProperty.Register(nameof(AnchorForeground), typeof(Brush), typeof(SplitViewSplitter), new PropertyMetadata(Application.Current.Resources["SystemControlForegroundBaseHighBrush"]));

		public double MinAnchorPosition
		{
			get { return (double)GetValue(MinAnchorPositionProperty); }
			set { SetValue(MinAnchorPositionProperty, value); }
		}

		public static readonly DependencyProperty MinAnchorPositionProperty =
			DependencyProperty.Register(nameof(MinAnchorPosition), typeof(double), typeof(SplitViewSplitter), new PropertyMetadata(0.0));

		public double MaxAnchorPosition
		{
			get { return (double)GetValue(MaxAnchorPositionProperty); }
			set { SetValue(MaxAnchorPositionProperty, value); }
		}

		public static readonly DependencyProperty MaxAnchorPositionProperty =
			DependencyProperty.Register("MaxAnchorPosition", typeof(double), typeof(SplitViewSplitter), new PropertyMetadata(double.MaxValue));

		public SplitViewSplitter()
		{
			InitializeComponent();

			_dragOffset = 0;
			_resetCursor = true;
		}

		private void PaneAnchor_ManipulationStarted(object _, ManipulationStartedRoutedEventArgs e)
		{
			_dragOffset = e.Position.X;
			_resetCursor = false;
			SetCursor(CoreCursorType.SizeWestEast);
		}

		private void PaneAnchor_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
		{
			var rect = sender as Rectangle;

			var transform = rect.TransformToVisual(PaneSplitter);
			var point = transform.TransformPoint(e.Position);
			var containerWidth = PaneSplitter.ActualWidth - rect.Width;
			var min = Math.Max(0, MinAnchorPosition);
			var max = Math.Min(containerWidth, MaxAnchorPosition);

			var left = Math.Clamp(point.X - _dragOffset, min, max);
			rect.Margin = new Thickness(left, rect.Margin.Top, rect.Margin.Right, rect.Margin.Bottom);

			PaneSplitter.OpenPaneLength = left;
		}

		private void PaneAnchor_ManipulationCompleted(object _, ManipulationCompletedRoutedEventArgs __)
		{
			_dragOffset = 0;
			_resetCursor = true;
			SetCursor(CoreCursorType.Arrow);
		}

		private Thickness ToLeftMargin(FrameworkElement sender, double margin)
		{
			return new Thickness(margin, sender.Margin.Top, sender.Margin.Right, sender.Margin.Bottom);
		}

		private void PaneAnchor_PointerEntered(object _, PointerRoutedEventArgs __)
		{
			SetCursor(CoreCursorType.SizeWestEast);
		}

		private void PaneAnchor_PointerMoved(object _, PointerRoutedEventArgs __)
		{
			SetCursor(CoreCursorType.SizeWestEast);
		}

		private void PaneAnchor_PointerExited(object _, PointerRoutedEventArgs __)
		{
			if (!_resetCursor) return;

			SetCursor(CoreCursorType.Arrow);
		}

		private void SetCursor(CoreCursorType cursor)
		{
			Window.Current.CoreWindow.PointerCursor = new CoreCursor(cursor, 0);
		}

		private double OffsetIcon(double anchorPosition)
		{
			return anchorPosition + 11.5;
		}

		private GridLength GetColumnWidth(double width)
		{
			return new GridLength(width);
		}
	}
}
