using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		Task<IReadOnlyList<Room>> GetRoomsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<User>> GetUsersForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<Message>> GetMessagesForRoomAsync(Room room);
		Task<IReadOnlyList<Message>> GetMessagesForUserAsync(User user);
		Task SendMessageToUserAsync(User user, string message);
		Task<UserProfile> GetUserProfileAsync(User user, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<ApiKeyInfo> GetApiKeyInfoAsync(string apiKey);
	}
}