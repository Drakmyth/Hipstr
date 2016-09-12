using Hipstr.Client.Views.Messages;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Hipstr.Client.Commands;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel : ViewModelBase, ITitled, IFilterable
	{
		private readonly IHipChatService _hipChatService;

		public string Title => "Rooms";
		public ObservableCollection<Room> Rooms { get; set; }
		public ObservableCollection<FilterItem> Filters { get; set; }
		public ICommand NavigateToMessagesViewCommand { get; }

		private Room _selectedRoom;
		public Room SelectedRoom
		{
			get { return _selectedRoom; }
			set
			{
				if (_selectedRoom == value) return;

				OnPropertyChanging();
				_selectedRoom = value;
				OnPropertyChanged();
				OnSelectedRoomChanged();
			}
		}

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public RoomsViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			NavigateToMessagesViewCommand = new NavigateToViewCommand<MessagesView>();

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

		private void OnSelectedRoomChanged()
		{
			if (NavigateToMessagesViewCommand.CanExecute(SelectedRoom))
			{
				NavigateToMessagesViewCommand.Execute(SelectedRoom);
			}
		}
	}
}