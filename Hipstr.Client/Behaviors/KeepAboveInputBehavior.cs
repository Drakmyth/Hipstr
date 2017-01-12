using Microsoft.Xaml.Interactivity;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;

namespace Hipstr.Client.Behaviors
{
	public class KeepAboveInputBehavior : Behavior<FrameworkElement>
	{
		private Thickness _originalMargin;

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Loaded += OnAssociatedObjectLoaded;
			_originalMargin = AssociatedObject.Margin;
		}

		protected override void OnDetaching()
		{
			InputPane.GetForCurrentView().Hiding -= OnInputPanelHiding;
			InputPane.GetForCurrentView().Showing -= OnInputPanelShowing;
		}

		private void OnAssociatedObjectLoaded(object sender, RoutedEventArgs e)
		{
			AssociatedObject.Loaded -= OnAssociatedObjectLoaded;
			InputPane.GetForCurrentView().Hiding += OnInputPanelHiding;
			InputPane.GetForCurrentView().Showing += OnInputPanelShowing;
		}

		private void OnInputPanelHiding(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			AssociatedObject.Margin = _originalMargin;
		}

		private void OnInputPanelShowing(InputPane sender, InputPaneVisibilityEventArgs args)
		{
			AssociatedObject.Margin = new Thickness(_originalMargin.Left, _originalMargin.Top, _originalMargin.Right, _originalMargin.Bottom + args.OccludedRect.Height);
		}
	}
}