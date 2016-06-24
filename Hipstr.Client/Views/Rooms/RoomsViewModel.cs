using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel
	{
		private readonly IHipChatService _hipChatService;

		public ObservableCollection<RoomSummary> Rooms { get; set; }

		public RoomsViewModel() : this(IoCContainer.Resolve<IHipChatService>()) { }

		public RoomsViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			UpdateRoomList();
		}

		public void UpdateRoomList()
		{
			IEnumerable<CollectionWrapper<RoomSummary>> wrappers = _hipChatService.GetRooms();

			List<RoomSummary> rooms = new List<RoomSummary>();
			foreach (CollectionWrapper<RoomSummary> wrapper in wrappers)
			{
				rooms.AddRange(wrapper.Items);
			}

			Rooms = new ObservableCollection<RoomSummary>(rooms);
		}
	}
}