using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Messaging
{
	public class RoomMessageSource : IMessageSource
	{
		public string Name => Room.Name;

		private readonly IHipChatService _hipChatService;
		public Room Room { get; }

		public RoomMessageSource(IHipChatService hipChatService, Room room)
		{
			_hipChatService = hipChatService;
			Room = room;
		}

		public void SendMessage()
		{
			throw new System.NotImplementedException();
		}

		public async Task<IReadOnlyList<Message>> GetMessagesAsync()
		{
			return await _hipChatService.GetMessagesForRoomAsync(Room);
		}
	}
}