using Hipstr.Core.Models.HipChat;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		CollectionWrapper<RoomSummary> GetRooms();
		CollectionWrapper<UserSummary> GetUsers();
	}
}