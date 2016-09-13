using Hipstr.Client.Commands;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;
using Hipstr.Core.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }
		public ObservableCollection<FilterItem> Filters { get; }

		private string _title;
		public string Title
		{
			get { return _title; }
			set
			{
				OnPropertyChanging();
				_title = value;
				OnPropertyChanged();
			}
		}

		private bool _showFilters;
		public bool ShowFilters
		{
			get { return _showFilters; }
			set
			{
				OnPropertyChanging();
				_showFilters = value;
				OnPropertyChanged();
			}
		}

		public MainPageViewModel()
		{
			NavigateToTeamsViewCommand = new NavigateToViewCommand<TeamsView>();
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>();
			NavigateToUsersViewCommand = new NavigateToViewCommand<UsersView>();
			Filters = new ObservableCollection<FilterItem>();
		}
	}
}