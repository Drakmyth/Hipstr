using Hipstr.Client.Commands.ViewCommands;
using Hipstr.Core.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml;
using Hipstr.Client.Views.Rooms;
using Hipstr.Client.Views.Teams;
using Hipstr.Client.Views.Users;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel : ViewModelBase
	{
		public ICommand NavigateToTeamsViewCommand { get; }
		public ICommand NavigateToRoomsViewCommand { get; }
		public ICommand NavigateToUsersViewCommand { get; }
		public ObservableCollection<FilterItem> Filters { get; }

		private FrameworkElement _frameContent;
		public FrameworkElement FrameContent
		{
			get { return _frameContent; }
			set
			{
				OnPropertyChanging();
				_frameContent = value;
				OnPropertyChanged();
				OnFrameContentChanged();
			}
		}

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

		private void OnFrameContentChanged()
		{
			ITitled titled = FrameContent.DataContext as ITitled;
			if (titled != null)
			{
				Title = titled.Title;
			}

			IFilterable filterable = FrameContent.DataContext as IFilterable;
			if (filterable != null)
			{
				Filters.Clear();
				filterable.Filters.ToList().ForEach(filter => Filters.Add(filter));
				ShowFilters = Filters.Any();
			}
		}
	}
}