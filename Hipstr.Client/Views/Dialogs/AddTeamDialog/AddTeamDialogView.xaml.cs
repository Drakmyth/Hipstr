using Hipstr.Client.Events;
using Hipstr.Core.Models;
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

namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
{
	public sealed partial class AddTeamDialogView : UserControl
	{
		private const string VisualStateTeamNameNormal = "TeamNameNormal";
		private const string VisualStateTeamNameErrored = "TeamNameErrored";
		private const string VisualStateApiKeyNormal = "ApiKeyNormal";
		private const string VisualStateApiKeyErrored = "ApiKeyErrored";

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;

		public AddTeamDialogViewModel ViewModel => DataContext as AddTeamDialogViewModel;

		public AddTeamDialogView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<AddTeamDialogViewModel>();

			_parent = new Popup {Child = this};
			ResizePopup();
			_parent.ChildTransitions = new TransitionCollection
			{
				new PopupThemeTransition
				{
					FromVerticalOffset = Height * 3
				}
			};
			_parent.IsLightDismissEnabled = false;

			ViewModel.Validation += OnValidation;
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

			VisualStateManager.GoToState(this, VisualStateTeamNameNormal, false);
		}

		private void ApiKeyTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				AcceptDialogButton.Focus(FocusState.Keyboard);
			}

			VisualStateManager.GoToState(this, VisualStateApiKeyNormal, false);
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			OnCancelled();
			e.Handled = true;
		}

		public IAsyncOperation<DialogResult<Team>> ShowAsync()
		{
			_parent.IsOpen = true;
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<DialogResult<Team>>();
				token.Register(OnCancelled);
				return _taskCompletionSource.Task;
			});
		}

		private void OnCancelled()
		{
			Hide();
			_taskCompletionSource.SetResult(DialogResult<Team>.CancelledResult());
		}

		private void Hide()
		{
			_parent.IsOpen = false;
		}

		private void CancelDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			OnCancelled();
		}

		private void OnValidation(object sender, AddTeamDialogValidationEventArgs e)
		{
			string teamNameState = e.TeamNameValidationResult.IsValid ? VisualStateTeamNameNormal : VisualStateTeamNameErrored;
			string apiKeyState = e.ApiKeyValidationResult.IsValid ? VisualStateApiKeyNormal : VisualStateApiKeyErrored;

			TeamNameErrorTextBlock.Text = e.TeamNameValidationResult.Reason;
			ApiKeyErrorTextBlock.Text = e.ApiKeyValidationResult.Reason;

			VisualStateManager.GoToState(this, teamNameState, false);
			VisualStateManager.GoToState(this, apiKeyState, false);

			if (!e.TeamNameValidationResult.IsValid || !e.ApiKeyValidationResult.IsValid) return;

			Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(ViewModel.TeamName, ViewModel.ApiKey)));
		}
	}
}