using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	[UsedImplicitly]
	public class HipChatService : IHipChatService
	{
		// TODO: Notify user when API_KEY is about to expire
		private const int MaxRoomResults = 1000;
		private const bool IncludePrivateRooms = true;
		private const bool IncludeArchivedRooms = true;

		private const int MaxUserResults = 1000;
		private const bool IncludeGuestUsers = true;
		private const bool IncludeDeletedUsers = false;

		private static readonly Uri RootUri = new Uri("http://www.hipchat.com");

		private readonly IDataService _dataService;
		private readonly IHttpClient _httpClient;

		public HipChatService(IDataService dataService, IHttpClient httpClient)
		{
			_dataService = dataService;
			_httpClient = httpClient;
		}

		public async Task<IReadOnlyList<Room>> GetRoomsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache)
		{
			switch (cacheBehavior)
			{
				case HipChatCacheBehavior.LoadFromCache:
					IReadOnlyList<Room> rooms = await _dataService.LoadRoomsForTeamAsync(team);
					if (!rooms.Any())
					{
						// room collection data wasn't cached, so we'll fetch it
						return await GetRoomsAndSaveToCacheAsync(team);
					}
					return rooms;
				case HipChatCacheBehavior.RefreshCache:
					return await GetRoomsAndSaveToCacheAsync(team);
				default:
					throw new ArgumentOutOfRangeException($"Unknown Cache Behavior - {cacheBehavior}", nameof(cacheBehavior));
			}
		}

		private async Task<IReadOnlyList<Room>> GetRoomsAndSaveToCacheAsync(Team team)
		{
			IReadOnlyList<Room> rooms = await GetRoomsForTeamFromServerAsync(team);
			await _dataService.SaveRoomsForTeamAsync(rooms, team);
			return rooms;
		}

		private async Task<IReadOnlyList<Room>> GetRoomsForTeamFromServerAsync(Team team)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);

			var rooms = new List<Room>();
			var loadAnotherPage = true;

			while (loadAnotherPage)
			{
				HttpResponseMessage response = await GetPageOfRooms(_httpClient, rooms.Count);
				string json = await response.Content.ReadAsStringAsync();
				var roomWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatRoom>>(json);

				rooms.AddRange(roomWrapper.Items.Select(hcRoom => new Room
				{
					Id = hcRoom.Id,
					IsArchived = hcRoom.IsArchived,
					Name = hcRoom.Name,
					Privacy = hcRoom.Privacy,
					Team = team
				}));

				loadAnotherPage = roomWrapper.Items.Count() == MaxRoomResults;
			}

			return rooms;
		}

		private static async Task<HttpResponseMessage> GetPageOfRooms(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/room?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxRoomResults}&"
						   + $"include-private={IncludePrivateRooms}&"
						   + $"include-archived={IncludeArchivedRooms}";

			return await httpClient.GetAsync(new Uri(RootUri, route));
		}

		public async Task<IReadOnlyList<User>> GetUsersForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache)
		{
			switch (cacheBehavior)
			{
				case HipChatCacheBehavior.LoadFromCache:
					IReadOnlyList<User> users = await _dataService.LoadUsersForTeamAsync(team);
					if (!users.Any())
					{
						// user collection data wasn't cached, so we'll fetch it
						return await GetUsersAndSaveToCacheAsync(team);
					}
					return users;
				case HipChatCacheBehavior.RefreshCache:
					return await GetUsersAndSaveToCacheAsync(team);
				default:
					throw new ArgumentOutOfRangeException($"Unknown Cache Behavior - {cacheBehavior}", nameof(cacheBehavior));
			}
		}

		private async Task<IReadOnlyList<User>> GetUsersAndSaveToCacheAsync(Team team)
		{
			IReadOnlyList<User> users = await GetUsersForTeamFromServerAsync(team);
			await _dataService.SaveUsersForTeamAsync(users, team);
			return users;
		}

		private async Task<IReadOnlyList<User>> GetUsersForTeamFromServerAsync(Team team)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);

			var users = new List<User>();
			var loadAnotherPage = true;

			while (loadAnotherPage)
			{
				HttpResponseMessage response = await GetPageOfUsers(_httpClient, users.Count);
				string json = await response.Content.ReadAsStringAsync();
				var userWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatUser>>(json);

				users.AddRange(userWrapper.Items.Select(hcUser => new User
				{
					Id = hcUser.Id,
					Handle = hcUser.MentionName,
					Name = hcUser.Name,
					Team = team
				}));

				loadAnotherPage = userWrapper.Items.Count() == MaxUserResults;
			}

			return users;
		}

		private static async Task<HttpResponseMessage> GetPageOfUsers(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/user?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxUserResults}&"
						   + $"include-guests={IncludeGuestUsers}&"
						   + $"include-deleted={IncludeDeletedUsers}";

			return await httpClient.GetAsync(new Uri(RootUri, route));
		}

		public async Task<IReadOnlyList<Message>> GetMessagesForRoomAsync(Room room)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/room/{room.Id}/history"));
			string json = await get.Content.ReadAsStringAsync();

			var messageWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<object>>(json);
			IEnumerable<JObject> jsonObjects = messageWrapper.Items.Cast<JObject>().Where(jobj => jobj.Value<string>("type") == "message").ToList();
			var hcMessages = JsonConvert.DeserializeObject<IEnumerable<HipChatMessage>>(JsonConvert.SerializeObject(jsonObjects));

			return hcMessages.Select(hcMessage => new Message
			{
				PostedBy = new User
				{
					Id = hcMessage.From.Id,
					Handle = hcMessage.From.MentionName,
					Name = hcMessage.From.Name,
					Team = room.Team
				},
				Date = hcMessage.Date,
				Text = hcMessage.Message
			}).ToList();
		}

		public async Task<UserProfile> GetUserProfileAsync(User user, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache)
		{
			switch (cacheBehavior)
			{
				case HipChatCacheBehavior.LoadFromCache:
					// TODO: Do we want to support caching of user profiles?
					return await GetUserProfileFromServerAsync(user);
				case HipChatCacheBehavior.RefreshCache:
					return await GetUserProfileFromServerAsync(user);
				default:
					throw new ArgumentOutOfRangeException($"Unknown Cache Behavior - {cacheBehavior}", nameof(cacheBehavior));
			}
		}

		private async Task<UserProfile> GetUserProfileFromServerAsync(User user)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Team.ApiKey);

			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/user/{user.Id}"));

			string json = await get.Content.ReadAsStringAsync();

			var hcUserProfile = JsonConvert.DeserializeObject<HipChatUserProfile>(json);

			return new UserProfile
			{
				Id = hcUserProfile.Id,
				Created = hcUserProfile.Created,
				Email = hcUserProfile.Email,
				IsDeleted = hcUserProfile.IsDeleted,
				IsGroupAdmin = hcUserProfile.IsGroupAdmin,
				IsGuest = hcUserProfile.IsGuest,
				LastActive = DateTime.Parse(hcUserProfile.LastActive),
				MentionName = hcUserProfile.MentionName,
				Name = hcUserProfile.Name,
				PhotoUrl = hcUserProfile.PhotoUrl,
				Title = hcUserProfile.Title
			};
		}

		public async Task<ApiKeyInfo> GetApiKeyInfoAsync(string apiKey)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/oauth/token/{apiKey}"));

			string json = await get.Content.ReadAsStringAsync();

			var hcSession = JsonConvert.DeserializeObject<HipChatOAuthSession>(json);

			return new ApiKeyInfo
			{
				Id = hcSession.Id,
				ApiKey = hcSession.AccessToken,
				RefreshToken = hcSession.RefreshToken,
				Scopes = hcSession.Scopes
			};
		}
	}
}