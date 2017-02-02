using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Hipstr.Client.Views.Dialogs.AddRoomDialog
{
	public sealed partial class AddRoomDialogView : UserControl
	{
		private const string VisualStateNameNormal = "NameNormal";
		private const string VisualStateNameErrored = "NameErrored";

		private readonly ParentPopupManager _popUp;
		private TaskCompletionSource<DialogResult<RoomCreationRequest>> _taskCompletionSource;

		public AddRoomDialogViewModel ViewModel => (AddRoomDialogViewModel)DataContext;

		private ICommand CancelDialogCommand { get; }

		public AddRoomDialogView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<AddRoomDialogViewModel>();

			_popUp = new ParentPopupManager(this);
			CancelDialogCommand = new RelayCommand(CancelDialog);

			ViewModel.Validation += OnValidation;
		}

		private void OnLoaded(object sender, RoutedEventArgs e)
		{
			_popUp.BindToWindow();
			ViewModel.Initialize();
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
		}

		private void OnUnloaded(object sender, RoutedEventArgs e)
		{
			_popUp.UnbindFromWindow();
			SystemNavigationManager.GetForCurrentView().BackRequested -= OnBackRequested;
			ViewModel.Dispose();
		}

		private void NameTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				TopicTextBox.Focus(FocusState.Keyboard);
			}

			VisualStateManager.GoToState(this, VisualStateNameNormal, false);
		}

		private void TopicTextBox_OnKeyUp(object sender, KeyRoutedEventArgs keyRoutedEventArgs)
		{
			if (keyRoutedEventArgs.Key == VirtualKey.Enter)
			{
				VisibleToDelegateAdminCheckBox.Focus(FocusState.Keyboard);
			}
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			CancelDialog();
			e.Handled = true;
		}

		private void OnValidation(object sender, AddRoomDialogValidationEventArgs e)
		{
			string nameState = e.NameValidationResult.IsValid ? VisualStateNameNormal : VisualStateNameErrored;

			NameErrorTextBlock.Text = e.NameValidationResult.Reason;

			VisualStateManager.GoToState(this, nameState, false);

			if (!e.NameValidationResult.IsValid) return;

			CloseDialog();
		}

		public IAsyncOperation<DialogResult<RoomCreationRequest>> ShowAsync(IReadOnlyList<Team> teams, Team defaultSelection = null)
		{
			ViewModel.Teams = new ObservableCollection<Team>(teams);
			ViewModel.SelectedTeam = defaultSelection ?? teams.First();

			_popUp.Show();
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<DialogResult<RoomCreationRequest>>();
				token.Register(CancelDialog);
				return _taskCompletionSource.Task;
			});
		}

		private void CloseDialog()
		{
			_popUp.Hide();

			_taskCompletionSource.SetResult(new DialogResult<RoomCreationRequest>(new RoomCreationRequest
			{
				Team = ViewModel.SelectedTeam,
				DelegateAdminVisibility = ViewModel.IsDelegateAdminVisibilityGroupDefault ? null : (bool?)ViewModel.IsVisibleToDelegateAdmin,
				GuestAccess = ViewModel.GuestAccessEnabled,
				Name = ViewModel.Name,
				Privacy = ViewModel.IsPrivate ? HipChatPrivacyTypes.Private : HipChatPrivacyTypes.Public,
				Topic = ViewModel.Topic
			}));
		}

		private void CancelDialog()
		{
			_popUp.Hide();
			_taskCompletionSource.SetResult(DialogResult<RoomCreationRequest>.CancelledResult());
		}

		private void Grid_OnPointerPressed(object sender, PointerRoutedEventArgs e)
		{
			AcceptDialogButton.Focus(FocusState.Programmatic);
		}
	}
}