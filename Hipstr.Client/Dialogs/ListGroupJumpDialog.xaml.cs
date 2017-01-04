using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Metadata;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Hipstr.Client.Views;
using Hipstr.Core.Utility.Extensions;

namespace Hipstr.Client.Dialogs
{
	public sealed partial class ListGroupJumpDialog : UserControl
	{
		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly ObservableCollection<JumpHeader> _groupHeaders;

		private readonly Popup _parent;
		private TaskCompletionSource<DialogResult<string>> _taskCompletionSource;

		// TODO: Commonize Dialog logic into a control baseclass or service
		public ListGroupJumpDialog()
		{
			InitializeComponent();
			_groupHeaders = new ObservableCollection<JumpHeader>();
			_parent = new Popup {Child = this};
			ResizePopup();
			_parent.IsLightDismissEnabled = false;

			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
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

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			Hide();
			e.Handled = true;
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

		public IAsyncOperation<DialogResult<string>> ShowAsync(IEnumerable<JumpHeader> headers)
		{
			_groupHeaders.Clear();
			_groupHeaders.AddRange(headers);

			_parent.IsOpen = true;
			return AsyncInfo.Run(WaitForInput);
		}

		private Task<DialogResult<string>> WaitForInput(CancellationToken token)
		{
			_taskCompletionSource = new TaskCompletionSource<DialogResult<string>>();
			token.Register(OnCancelled);
			return _taskCompletionSource.Task;
		}

		private void OnCancelled()
		{
			Hide();
			_taskCompletionSource.SetResult(DialogResult<string>.CancelledResult());
		}

		private void Hide()
		{
			_parent.IsOpen = false;
		}

		private void HeaderTextBlock_OnTapped(object sender, TappedRoutedEventArgs e)
		{
			var tapped = (TextBlock)sender;
			JumpHeader header = _groupHeaders.Where(jh => jh.Header == tapped.Text).Single();

			if (!header.Enabled) return;

			Hide();
			_taskCompletionSource.SetResult(new DialogResult<string>(tapped.Text));
		}
	}
}