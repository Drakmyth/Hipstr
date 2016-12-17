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
				_showFilters = value;
				OnPropertyChanged();
			}
		}

		private readonly IMainPageService _mainPageService;

		public MainPageViewModel() : this(IoCContainer.Resolve<IMainPageService>())
		{
		}

		public MainPageViewModel(IMainPageService mainPageService)
		{
			_mainPageService = mainPageService;
			_mainPageService.TitleChanged += OnTitleChanged;
			Title = "";
			NavigateToTeamsViewCommand = new NavigateToViewCommand<TeamsView>();
			NavigateToRoomsViewCommand = new NavigateToViewCommand<RoomsView>();
			NavigateToUsersViewCommand = new NavigateToViewCommand<UsersView>();
			Filters = new ObservableCollection<FilterItem>();
		}

		private void OnTitleChanged(object sender, string title)
		{
			Title = title;
		}
	}
}