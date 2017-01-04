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
using Windows.UI.Xaml.Media.Animation;
using Hipstr.Core.Models;

namespace Hipstr.Client.Views.Dialogs.EditTeamDialog
{
	public sealed partial class EditTeamDialogView : UserControl
	{
		private string _teamName;
		private string _apiKey;

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<DialogResult<Team>> _taskCompletionSource;

		// TODO: Commonize Dialog logic into a control baseclass or service
		public EditTeamDialogView()
		{
			InitializeComponent();
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

		public IAsyncOperation<DialogResult<Team>> ShowAsync(Team team)
		{
			_teamName = team.Name;
			_apiKey = team.ApiKey;

			_parent.IsOpen = true;
			return AsyncInfo.Run(WaitForInput);
		}

		private Task<DialogResult<Team>> WaitForInput(CancellationToken token)
		{
			_taskCompletionSource = new TaskCompletionSource<DialogResult<Team>>();
			token.Register(OnCancelled);
			return _taskCompletionSource.Task;
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
			Hide();
			_taskCompletionSource.SetResult(DialogResult<Team>.CancelledResult());
		}

		private void AcceptDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(_teamName, _apiKey)));
		}
	}
}