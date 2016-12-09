using Hipstr.Core.Models;
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

namespace Hipstr.Client.Views.Teams
{
	public sealed partial class EditTeamDialog : UserControl
	{
		private string _teamName;
		public string TeamName => _teamName;

		private string _apiKey;
		public string ApiKey => _apiKey;

		private static ApplicationView Window => ApplicationView.GetForCurrentView();

		private readonly Popup _parent;
		private TaskCompletionSource<ModalResult<Team>> _taskCompletionSource;

		// TODO: Commonize Dialog logic into a control baseclass or service
		public EditTeamDialog(Team team)
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

			_teamName = team.Name;
			_apiKey = team.ApiKey;

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

		public IAsyncOperation<ModalResult<Team>> ShowAsync()
		{
			_parent.IsOpen = true;
			return AsyncInfo.Run(WaitForInput);
		}

		private Task<ModalResult<Team>> WaitForInput(CancellationToken token)
		{
			_taskCompletionSource = new TaskCompletionSource<ModalResult<Team>>();
			token.Register(OnCancelled);
			return _taskCompletionSource.Task;
		}

		private void OnCancelled()
		{
			Hide();
			_taskCompletionSource.SetResult(ModalResult<Team>.CancelledResult());
		}

		private void Hide()
		{
			_parent.IsOpen = false;
		}

		private void CancelDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_taskCompletionSource.SetResult(ModalResult<Team>.CancelledResult());
		}

		private void AcceptDialogButton_OnClick(object sender, RoutedEventArgs e)
		{
			Hide();
			_taskCompletionSource.SetResult(new ModalResult<Team>(new Team(_teamName, _apiKey)));
		}
	}
}