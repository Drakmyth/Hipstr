using Hipstr.Client.Commands;
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

namespace Hipstr.Client.Views.Dialogs.EditTeamDialog
{
	public sealed partial class EditTeamDialogView : UserControl
	{
		private const string VisualStateTeamNameNormal = "TeamNameNormal";
		private const string VisualStateTeamNameErrored = "TeamNameErrored";

		private readonly ParentPopupManager _popUp;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;

		public EditTeamDialogViewModel ViewModel => (EditTeamDialogViewModel)DataContext;

		private ICommand CancelDialogCommand { get; }

		public EditTeamDialogView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<EditTeamDialogViewModel>();

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
			ViewModel.Dispose();
		}

		private void TeamNameTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				AcceptDialogButton.Focus(FocusState.Keyboard);
			}

			VisualStateManager.GoToState(this, VisualStateTeamNameNormal, false);
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			CancelDialog();
			e.Handled = true;
		}

		private void OnValidation(object sender, EditTeamDialogValidationEventArgs e)
		{
			string teamNameState = e.TeamNameValidationResult.IsValid ? VisualStateTeamNameNormal : VisualStateTeamNameErrored;

			TeamNameErrorTextBlock.Text = e.TeamNameValidationResult.Reason;

			VisualStateManager.GoToState(this, teamNameState, false);

			if (!e.TeamNameValidationResult.IsValid) return;

			CloseDialog();
		}

		public IAsyncOperation<DialogResult<Team>> ShowAsync(Team team)
		{
			ViewModel.TeamName = team.Name;
			ViewModel.ApiKey = team.ApiKey;

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