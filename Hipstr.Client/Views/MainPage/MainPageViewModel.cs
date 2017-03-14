using Hipstr.Client.Commands;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Settings;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;
using JetBrains.Annotations;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	[UsedImplicitly]
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }
		public ICommand NavigateToSettingsViewCommand { get; }

		private bool _menuIsOpen;

		public bool MenuIsOpen
		{
			get { return _menuIsOpen; }
			set
			{
				_menuIsOpen = value;
				OnPropertyChanged();
			}
		}


		public MainPageViewModel()
		{
			_menuIsOpen = false;

			NavigateToTeamsViewCommand = new NavigateToViewCommand<TeamsView>
			{
				ClearBackStackOnNavigate = true
			};
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>
			{
				ClearBackStackOnNavigate = true,
				BackToSpecificType = typeof(TeamsView),
				ClearNavigationCache = true
			};
			NavigateToUsersViewCommand = new NavigateToViewCommand<UsersView>
			{
				ClearBackStackOnNavigate = true,
				BackToSpecificType = typeof(TeamsView),
				ClearNavigationCache = true
			};
			NavigateToSettingsViewCommand = new NavigateToViewCommand<SettingsView>();
		}
	}
}