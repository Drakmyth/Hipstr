using Hipstr.Client.Commands.ViewCommands;
using System;
using System.Windows.Input;
using Windows.UI.Core;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel
	{
		public event EventHandler<BackRequestedEventArgs> BackRequested;

		public ICommand NavigateToTeamsViewCommand { get; set; }
		public ICommand NavigateToRoomsViewCommand { get; set; }
		public ICommand NavigateToUsersViewCommand { get; set; }

		public MainPageViewModel()
		{
			NavigateToTeamsViewCommand = new NavigateToTeamsViewCommand();
			NavigateToRoomsViewCommand = new NavigateToRoomsViewCommand();
			NavigateToUsersViewCommand = new NavigateToUsersViewCommand();
			SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;
		}

		private void OnBackRequested(object sender, BackRequestedEventArgs e)
		{
			//			BackRequested?.Invoke(this, e);
			//
			//			if (e.Handled) return;
			//
			//			Frame rootFrame = (Frame)WindowCurrent.Content;
			//			if (rootFrame.CanGoBack)
			//			{
			//				e.Handled = true;
			//				rootFrame.GoBack();
			//			}
		}
	}
}