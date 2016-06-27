using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using System.Collections.Generic;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		IEnumerable<Room> GetRooms();
		HipChatCollectionWrapper<HipChatUser> GetUsers();
	}
}