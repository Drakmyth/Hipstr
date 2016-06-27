using Hipstr.Core.Models;
using System.Collections.Generic;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		IEnumerable<Room> GetRooms();
		IEnumerable<User> GetUsers();
	}
}