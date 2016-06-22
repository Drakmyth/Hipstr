using Hipstr.Core.Models.HipChat;
using Hipstr.Core.Services;
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
			CollectionWrapper<RoomSummary> wrapper = _hipChatService.GetRooms();
			Rooms = new ObservableCollection<RoomSummary>(wrapper.Items);
		}
	}
}