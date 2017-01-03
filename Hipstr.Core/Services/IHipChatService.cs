﻿using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHipChatService
	{
		Task<IReadOnlyList<Room>> GetRoomsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<User>> GetUsersForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache);
		Task<IReadOnlyList<Message>> GetMessagesAsync(Room room);
		Task<UserProfile> GetUserProfileAsync(User user);
	}
}