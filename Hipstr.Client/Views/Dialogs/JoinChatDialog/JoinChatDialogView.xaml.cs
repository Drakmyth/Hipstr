using Hipstr.Client.Commands;
using Hipstr.Client.Utility;
using Hipstr.Core.Messaging;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Dialogs.JoinChatDialog
{
	public sealed partial class JoinChatDialogView : UserControl
	{
		private readonly ParentPopupManager _popUp;
		private TaskCompletionSource<DialogResult<IMessageSource>> _taskCompletionSource;

		public JoinChatDialogViewModel ViewModel => (JoinChatDialogViewModel)DataContext;

        private string Title { get; }
		private ICommand ConfirmDialogCommand { get; }
		private ICommand CancelDialogCommand { get; }

		public JoinChatDialogView(string title)
		{
            Title = title;

			InitializeComponent();
			DataContext = IoCContainer.Resolve<JoinChatDialogViewModel>();

			_popUp = new ParentPopupManager(this);
			ConfirmDialogCommand = new RelayCommand(CloseDialog, () => ViewModel.SelectedMessageSource != null, ViewModel, nameof(ViewModel.SelectedMessageSource));
			CancelDialogCommand = new RelayCommand(CancelDialog);
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

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			CancelDialog();
			e.Handled = true;
		}

		public IAsyncOperation<DialogResult<IMessageSource>> ShowAsync(IReadOnlyList<IMessageSource> messageSources)
		{
			IEnumerable<MessageSourceGroup> groupedSources =
				messageSources.GroupBy(
					GroupNameSelectors.StandardGroupNameSelector,
					(key, sources) => new MessageSourceGroup(key, sources.OrderBy(ums => ums.Name)));
			ViewModel.MessageSources.Clear();
			ViewModel.MessageSources.AddRange(groupedSources.OrderBy(msg => msg.Key));

			_popUp.Show();
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<DialogResult<IMessageSource>>();
				token.Register(CancelDialog);
				return _taskCompletionSource.Task;
			});
		}

		private void CloseDialog()
		{
			_popUp.Hide();

			_taskCompletionSource.SetResult(new DialogResult<IMessageSource>(ViewModel.SelectedMessageSource));
		}

		private void CancelDialog()
		{
			_popUp.Hide();
			_taskCompletionSource.SetResult(DialogResult<IMessageSource>.CancelledResult());
		}
	}
}