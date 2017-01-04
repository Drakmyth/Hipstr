using Windows.Foundation.Metadata;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media.Animation;

namespace Hipstr.Client.Views.Dialogs
{
	public class ParentPopupManager
	{
		private static ApplicationView Window => ApplicationView.GetForCurrentView();
		private FrameworkElement Child { get; }

		private readonly Popup _parent;

		public ParentPopupManager(FrameworkElement child)
		{
			_parent = new Popup {Child = child};
			Child = child;

			ResizePopup();
			_parent.ChildTransitions = new TransitionCollection
			{
				new PopupThemeTransition
				{
					FromVerticalOffset = Child.Height * 3
				}
			};
			_parent.IsLightDismissEnabled = false;
		}

		public void BindToWindow()
		{
			Window.VisibleBoundsChanged += OnVisibleBoundsChanged;
		}

		public void UnbindFromWindow()
		{
			Window.VisibleBoundsChanged -= OnVisibleBoundsChanged;
		}

		private void OnVisibleBoundsChanged(ApplicationView sender, object args)
		{
			ResizePopup();
		}

		private void ResizePopup()
		{
			Child.Width = Window.VisibleBounds.Width;

			double topMargin = 0;
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				topMargin = StatusBar.GetForCurrentView().OccludedRect.Height;
			}

			Child.Height = Window.VisibleBounds.Height;
			Child.Margin = new Thickness(0, topMargin, 0, 0);
		}

		public void Show()
		{
			_parent.IsOpen = true;
		}

		public void Hide()
		{
			_parent.IsOpen = false;
		}
	}
}