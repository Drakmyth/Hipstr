using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<Room> Rooms { get; set; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }
		public RoomsViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateRoomList();
		}

		public void UpdateRoomList()
		{
			IEnumerable<Room> rooms = _hipChatService.GetRooms();
			Rooms = new ObservableCollection<Room>(rooms);
		}
	}
}