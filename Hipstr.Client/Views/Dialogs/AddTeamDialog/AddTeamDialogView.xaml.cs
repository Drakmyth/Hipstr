using Hipstr.Client.Commands;
using Hipstr.Client.Events;
using Hipstr.Core.Models;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
{
	public sealed partial class AddTeamDialogView : UserControl
	{
		private const string VisualStateTeamNameNormal = "TeamNameNormal";
		private const string VisualStateTeamNameErrored = "TeamNameErrored";
		private const string VisualStateApiKeyNormal = "ApiKeyNormal";
		private const string VisualStateApiKeyErrored = "ApiKeyErrored";

		private readonly ParentPopupManager _popUp;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;

		public AddTeamDialogViewModel ViewModel => (AddTeamDialogViewModel)DataContext;

		private ICommand CancelDialogCommand { get; }

		public AddTeamDialogView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<AddTeamDialogViewModel>();

			_popUp = new ParentPopupManager(this);
			CancelDialogCommand = new RelayCommand(CancelDialog);

			ViewModel.Validation += OnValidation;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_popUp.BindToWindow();
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_popUp.UnbindFromWindow();
			SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
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
			CancelDialog();
			e.Handled = true;
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

			CloseDialog();
		}

		public IAsyncOperation<DialogResult<Team>> ShowAsync()
		{
			_popUp.Show();
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<DialogResult<Team>>();
				token.Register(CancelDialog);
				return _taskCompletionSource.Task;
			});
		}

		private void CloseDialog()
		{
			_popUp.Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(ViewModel.TeamName, ViewModel.ApiKey)));
		}

		private void CancelDialog()
		{
			_popUp.Hide();
			_taskCompletionSource.SetResult(DialogResult<Team>.CancelledResult());
		}

		private void Grid_OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			AcceptDialogButton.Focus(FocusState.Programmatic);
		}
	}
}