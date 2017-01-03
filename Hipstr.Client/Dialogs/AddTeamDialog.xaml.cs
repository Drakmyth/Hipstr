using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Hipstr.Client.Views;
using Hipstr.Core.Models;

namespace Hipstr.Client.Dialogs
{
	public sealed partial class AddTeamDialog : UserControl
	{
		private string _teamName;
		private string _apiKey;

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<ModalResult<Team>> _taskCompletionSource;

		// TODO: Commonize Dialog logic into a control baseclass or service
		public AddTeamDialog()
		{
			InitializeComponent();
			_parent = new Popup { Child = this };
			ResizePopup();
			_parent.ChildTransitions = new TransitionCollection
			{
				new PopupThemeTransition
				{
					FromVerticalOffset = Height * 3
				}
			};
			_parent.IsLightDismissEnabled = false;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			Window.VisibleBoundsChanged += OnVisibleBoundsChanged;
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			Window.VisibleBoundsChanged -= OnVisibleBoundsChanged;
			SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
		}

		private void OnVisibleBoundsChanged(ApplicationView sender, object args)
		{
			ResizePopup();
		}

		private void ResizePopup()
		{
			Width = Window.VisibleBounds.Width;

			double topMargin = 0;
			if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
			{
				topMargin = StatusBar.GetForCurrentView().OccludedRect.Height;
			}

			Height = Window.VisibleBounds.Height;
			Margin = new Thickness(0, topMargin, 0, 0);
		}

		private void TeamNameTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				ApiKeyTextBox.Focus(FocusState.Keyboard);
			}
		}

		private void ApiKeyTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				AcceptDialogButton.Focus(FocusState.Keyboard);
			}
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			OnCancelled();
			e.Handled = true;
		}

		public IAsyncOperation<ModalResult<Team>> ShowAsync()
		{
			_parent.IsOpen = true;
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<ModalResult<Team>>();
				token.Register(OnCancelled);
				return _taskCompletionSource.Task;
			});
		}

		private void OnCancelled()
		{
			Hide();
			_taskCompletionSource.SetResult(ModalResult<Team>.CancelledResult());
		}

		private void Hide()
		{
			_parent.IsOpen = false;
		}

		private void CancelDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			OnCancelled();
		}

		private void AcceptDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_taskCompletionSource.SetResult(new ModalResult<Team>(new Team(_teamName, _apiKey)));
		}
	}
}