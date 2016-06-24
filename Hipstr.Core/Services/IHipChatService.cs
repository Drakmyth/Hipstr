using Hipstr.Core.Models.HipChat;
using System.Collections.Generic;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		IEnumerable<CollectionWrapper<RoomSummary>> GetRooms();
		CollectionWrapper<UserSummary> GetUsers();
	}
}