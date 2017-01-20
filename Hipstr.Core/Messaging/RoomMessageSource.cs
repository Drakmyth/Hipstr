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
		public Team Team => Room.Team;

		public RoomMessageSource(IHipChatService hipChatService, Room room)
		{
			_hipChatService = hipChatService;
			Room = room;
		}

		public async Task SendMessageAsync(string message)
		{
			await _hipChatService.SendMessageToRoomAsync(Room, message);
		}

		public async Task<IReadOnlyList<Message>> GetMessagesAsync()
		{
			return await _hipChatService.GetMessagesForRoomAsync(Room);
		}
	}
}