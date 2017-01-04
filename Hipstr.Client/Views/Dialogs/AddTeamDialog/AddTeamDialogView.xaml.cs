using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;

namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
{
	public sealed partial class AddTeamDialogView : UserControl, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		private string _teamName;

		private string TeamName
		{
			get { return _teamName; }
			set
			{
				_teamName = value;
				OnPropertyChanged();
			}
		}

		private string _apiKey;

		private string ApiKey
		{
			get { return _apiKey; }
			set
			{
				_apiKey = value;
				OnPropertyChanged();
			}
		}

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;

		private ValidationResult _teamNameValidation;

		private ValidationResult TeamNameValidation
		{
			get { return _teamNameValidation; }
			set
			{
				_teamNameValidation = value;
				OnPropertyChanged();
			}
		}

		private ValidationResult _apiKeyValidation;

		private ValidationResult ApiKeyValidation
		{
			get { return _apiKeyValidation; }
			set
			{
				_apiKeyValidation = value;
				OnPropertyChanged();
			}
		}

		// TODO: Commonize Dialog logic into a control baseclass or service
		public AddTeamDialogView(ITeamService teamService, IHipChatService hipChatService)
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

			_teamNameValidation = ValidationResult.Valid();
			_apiKeyValidation = ValidationResult.Valid();
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

		private static ValidationResult ValidateTeamName(string teamName)
		{
			return string.IsNullOrWhiteSpace(teamName) ? ValidationResult.Invalid("Team Name is required") : ValidationResult.Valid();
		}

		private async Task<ValidationResult> ValidateApiKeyAsync(string apiKey)
		{
			IReadOnlyList<Team> teams = await _teamService.GetTeamsAsync();
			if (teams.Where(team => team.ApiKey == apiKey).Any())
			{
				return ValidationResult.Invalid("Provided API Key already registered");
			}

			try
			{
				await _hipChatService.GetApiKeyInfoAsync(apiKey);
			}
			catch
			{
				return ValidationResult.Invalid("Error validating provided API Key with HipChat server");
			}

			return ValidationResult.Valid();
		}

		private void UpdateVisualStates()
		{
			string teamNameState = TeamNameValidation.IsValid ? "TeamNameNormal" : "TeamNameErrored";
			string apiKeyState = ApiKeyValidation.IsValid ? "ApiKeyNormal" : "ApiKeyErrored";

			VisualStateManager.GoToState(this, teamNameState, false);
			VisualStateManager.GoToState(this, apiKeyState, false);
		}

		private async void AcceptDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			ValidationResult teamNameValidation = ValidateTeamName(_teamName);
			ValidationResult apiKeyValidation = await ValidateApiKeyAsync(_apiKey);

			TeamNameValidation = teamNameValidation;
			ApiKeyValidation = apiKeyValidation;

			UpdateVisualStates();

			if (!teamNameValidation.IsValid || !apiKeyValidation.IsValid) return;

			Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(TeamName, ApiKey)));
		}

		[NotifyPropertyChangedInvocator]
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}