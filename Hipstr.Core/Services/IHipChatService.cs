using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		Task<Room> CreateRoomForTeamAsync(Team team, RoomCreationRequest request);
		Task<IReadOnlyList<Room>> GetRoomsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<User>> GetUsersForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<Emoticon>> GetEmoticonsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<Emoticon> GetSingleEmoticon(string shortcut, Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<Message>> GetMessagesForRoomAsync(Room room);
		Task<IReadOnlyList<Message>> GetMessagesForUserAsync(User user);
		Task SendMessageToRoomAsync(Room room, string message);
		Task SendMessageToUserAsync(User user, string message);
		Task<UserProfile> GetUserProfileAsync(User user, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<ApiKeyInfo> GetApiKeyInfoAsync(string apiKey);
	}
}