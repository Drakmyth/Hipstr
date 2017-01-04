using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Hipstr.Client.Views.Dialogs.ListGroupJumpDialog
{
	public sealed partial class ListGroupJumpDialogView : UserControl
	{
		private readonly ParentPopupManager _popUp;
		private TaskCompletionSource<DialogResult<string>> _taskCompletionSource;

		private ObservableCollection<JumpHeader> GroupHeaders { get; }

		public ListGroupJumpDialogView()
		{
			InitializeComponent();

			GroupHeaders = new ObservableCollection<JumpHeader>();
			_popUp = new ParentPopupManager(this, ParentPopupTransitionType.Fade);
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

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			CancelDialog();
			e.Handled = true;
		}

		public IAsyncOperation<DialogResult<string>> ShowAsync(IEnumerable<JumpHeader> groupHeaders)
		{
			GroupHeaders.AddRange(groupHeaders);
			_popUp.Show();
			return AsyncInfo.Run(token =>
			{
				_taskCompletionSource = new TaskCompletionSource<DialogResult<string>>();
				token.Register(CancelDialog);
				return _taskCompletionSource.Task;
			});
		}

		private void CancelDialog()
		{
			_popUp.Hide();
			_taskCompletionSource.SetResult(DialogResult<string>.CancelledResult());
		}

		private void HeaderTextBlock_OnTapped(object sender, TappedRoutedEventArgs e)
		{
			var tapped = (TextBlock)sender;
			JumpHeader header = GroupHeaders.Where(jh => jh.Header == tapped.Text).Single();

			if (!header.Enabled) return;

			_popUp.Hide();
			_taskCompletionSource.SetResult(new DialogResult<string>(tapped.Text));
		}
	}
}