using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel
	{
		private readonly IHipChatService _hipChatService;

		public List<Room> Rooms { get; set; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }

		public RoomsViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateRoomList();
		}

		public void UpdateRoomList()
		{
			CollectionWrapper<RoomSummary> wrapper = _hipChatService.GetRooms();
			List<Room> rooms = wrapper.Items.Select(roomSummary => new Room(roomSummary.Name)).ToList();
			Rooms = rooms;
		}
	}
}