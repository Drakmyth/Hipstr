using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel : ViewModelBase, ITitled, IFilterable
	{
		private readonly IHipChatService _hipChatService;

		public string Title => "Rooms";
		public ObservableCollection<Room> Rooms { get; set; }

		private ObservableCollection<FilterItem> _filters;
		public ObservableCollection<FilterItem> Filters
		{
			get
			{
				return _filters;
			}
			set
			{
				OnPropertyChanging();
				_filters = value;
				OnPropertyChanged();
			}
		}

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public RoomsViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			UpdateRooms();
			UpdateFilters();
		}

		public void UpdateRooms()
		{
			IEnumerable<Room> rooms = _hipChatService.GetRooms();
			Rooms = new ObservableCollection<Room>(rooms);
		}

		public void UpdateFilters()
		{
			IEnumerable<FilterItem> filters = Rooms.GroupBy(room => room.Team.Name).Select(group => new FilterItem { DisplayName = group.Key }).ToList();
			Filters = new ObservableCollection<FilterItem>(filters);
		}
	}
}