using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;
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

namespace Hipstr.Client.Dialogs
{
	public sealed partial class AddTeamDialog : UserControl
	{
		private string _teamName;
		private string _apiKey;

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;
		private bool _apiKeyIsErrored;

		// TODO: Commonize Dialog logic into a control baseclass or service
		public AddTeamDialog(ITeamService teamService, IHipChatService hipChatService)
		{
			InitializeComponent();

			_teamService = teamService;
			_hipChatService = hipChatService;

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
			_apiKeyIsErrored = false;
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

		private async void AcceptDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			IReadOnlyList<Team> teams = await _teamService.GetTeamsAsync();
			if (teams.Where(team => team.ApiKey == _apiKey).Any())
			{
				VisualStateManager.GoToState(this, "ApiKeyErrored", false);
				// TODO: Handle duplicate Api Key
				_apiKeyIsErrored = true;
			}

			try
			{
				await _hipChatService.GetApiKeyInfoAsync(_apiKey);
			}
			catch
			{
				VisualStateManager.GoToState(this, "ApiKeyErrored", false);
				// TODO: Handle invalid Api Key
				_apiKeyIsErrored = true;
			}

			if (_apiKeyIsErrored) return;

			Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(_teamName, _apiKey)));
		}
	}
}