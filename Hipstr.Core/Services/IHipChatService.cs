using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		Task<IEnumerable<Room>> GetRoomsAsync();
		Task<IEnumerable<User>> GetUsersAsync();
		Task<IEnumerable<Message>> GetMessagesAsync(Room room);
		Task<UserProfile> GetUserProfileAsync(User user);
	}
}