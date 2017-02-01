using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
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

		private const int MaxEmoticonResults = 1000;
		private const string FilterEmoticonType = "all";
		private const int PrecachedEmoticonDimensions = 30;

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

		public async Task SendMessageToRoomAsync(Room room, string message)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			var payload = new HipChatMessageToRoom
			{
				Message = message
			};
			await _httpClient.PostAsync(new Uri(RootUri, $"/v2/room/{room.Id}/message"), payload);
		}

		public async Task<IReadOnlyList<Message>> GetMessagesForRoomAsync(Room room)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/room/{room.Id}/history"));
			string json = await get.Content.ReadAsStringAsync();

			var messageWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<object>>(json);
			// TODO: Deserialize each message individually based on type
			IEnumerable<JObject> jsonObjects = messageWrapper.Items.Cast<JObject>().Where(jobj => jobj.Value<string>("type") == HipChatMessageTypes.Message).ToList();
			var hcMessages = JsonConvert.DeserializeObject<IEnumerable<HipChatMessage>>(JsonConvert.SerializeObject(jsonObjects));

			IList<Message> messages = hcMessages.Select(hcMessage => BuildMessage(room.Team, hcMessage)).ToList();

			return ParseMessages(messages);
		}

		private static Message BuildMessage(Team team, HipChatMessage hcMessage)
		{
			IMessageBuilder messageBuilder = Message.Builder(
				new User
				{
					Id = hcMessage.From.Id,
					Handle = hcMessage.From.MentionName,
					Name = hcMessage.From.Name,
					Team = team
				},
				hcMessage.Date,
				hcMessage.Message);

			if (hcMessage.MessageLinks == null) return messageBuilder.Build();

			IEnumerable<JObject> messageLinks = hcMessage.MessageLinks.Cast<JObject>().ToList();

			IEnumerable<JObject> imageJObjects = messageLinks.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Image).ToList();
			IEnumerable<JObject> linkJObjects = messageLinks.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Link).ToList();
			IEnumerable<JObject> twitterUserJObjects = messageLinks.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.TwitterUser).ToList();
			IEnumerable<JObject> twitterStatusJObjects = messageLinks.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.TwitterStatus).ToList();
			IEnumerable<JObject> videoJObjects = messageLinks.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Video).ToList();

			var images = JsonConvert.DeserializeObject<IEnumerable<MessageImage>>(JsonConvert.SerializeObject(imageJObjects));
			var links = JsonConvert.DeserializeObject<IEnumerable<MessageLink>>(JsonConvert.SerializeObject(linkJObjects));
			var twitterUsers = JsonConvert.DeserializeObject<IEnumerable<MessageTwitterUser>>(JsonConvert.SerializeObject(twitterUserJObjects));
			var twitterStatuses = JsonConvert.DeserializeObject<IEnumerable<MessageTwitterStatus>>(JsonConvert.SerializeObject(twitterStatusJObjects));
			var videos = JsonConvert.DeserializeObject<IEnumerable<MessageVideo>>(JsonConvert.SerializeObject(videoJObjects));

			return messageBuilder.WithImages(images)
				.WithLinks(links)
				.WithTwitterUsers(twitterUsers)
				.WithTwitterStatuses(twitterStatuses)
				.WithVideos(videos)
				.Build();
		}

		private static IReadOnlyList<Message> ParseMessages(IList<Message> messages)
		{
			IList<Message> parsedMessages = new List<Message>();

			for (var i = 0; i < messages.Count; i++)
			{
				Message currentMessage = messages[i];
				if (i == messages.Count - 1)
				{
					parsedMessages.Add(currentMessage);
					continue;
				}

				Message nextMessage = messages[i + 1];
				Message editedMessage = ParseForSedReplacement(currentMessage, nextMessage);
				if (editedMessage.Edits.Contains(nextMessage))
				{
					messages.Remove(nextMessage);
					messages[i] = editedMessage;
					i--;
					continue;
				}
				parsedMessages.Add(editedMessage);
			}

			return new ReadOnlyCollection<Message>(parsedMessages);
		}

		private static Message ParseForSedReplacement(Message originalMessage, Message replacementMessage)
		{
			if (originalMessage.PostedBy.Id != replacementMessage.PostedBy.Id) return originalMessage;
			if (replacementMessage.Date - originalMessage.Date > new TimeSpan(0, 30, 0)) return originalMessage;

			int slashCount = replacementMessage.Text.Where(x => x == '/').Count();

			if (slashCount < 2) return originalMessage;
			if (slashCount == 3 && !replacementMessage.Text.EndsWith("/")) return originalMessage;
			if (slashCount > 3) return originalMessage;

			var sedRegex = new Regex(@"\As\/([^\/]+)\/([^\/]*)\/?$");
			Match match = sedRegex.Match(replacementMessage.Text);

			if (match.Groups.Count != 3) return originalMessage;

			string originalSlug = match.Groups[1].Value;
			string replacementSlug = match.Groups[2].Value;

			string editedText = originalMessage.Text.Replace(originalSlug, replacementSlug);

			IList<Message> edits = new List<Message>(originalMessage.Edits);
			edits.Add(replacementMessage);

			return Message.Builder(originalMessage)
				.WithText(editedText)
				.WithEdits(edits)
				.Build();
		}

		public async Task SendMessageToUserAsync(User user, string message)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Team.ApiKey);
			var payload = new HipChatMessageToUser
			{
				Message = message,
				Notify = true,
				MessageFormat = "text"
			};
			await _httpClient.PostAsync(new Uri(RootUri, $"/v2/user/{user.Id}/message"), payload);
		}

		public async Task<IReadOnlyList<Message>> GetMessagesForUserAsync(User user)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Team.ApiKey);
			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/user/{user.Id}/history"));
			string json = await get.Content.ReadAsStringAsync();

			var messageWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<object>>(json);
			IEnumerable<JObject> jsonObjects = messageWrapper.Items.Cast<JObject>().Where(jobj => jobj.Value<string>("type") == "message").ToList();
			var hcMessages = JsonConvert.DeserializeObject<IEnumerable<HipChatMessage>>(JsonConvert.SerializeObject(jsonObjects));

			IList<Message> messages = hcMessages.Select(hcMessage => BuildMessage(user.Team, hcMessage)).ToList();

			return ParseMessages(messages);
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

		public async Task<Emoticon> GetSingleEmoticon(string shortcut, Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache)
		{
			IReadOnlyList<Emoticon> emoticons = await _dataService.LoadEmoticonsForTeamAsync(team);
			Emoticon emoticon = emoticons.Where(e => e.Shortcut == shortcut).SingleOrDefault();

			if (emoticon == null)
			{
				// the requested emoticon didn't exist last time we cached, so we'll refresh
				emoticons = await GetEmoticonsAndSaveToCacheAsync(team);
				emoticon = emoticons.Where(e => e.Shortcut == shortcut).SingleOrDefault();

				if (emoticon == null)
				{
					// emoticon still doesn't exist
					return null;
				}
			}

			// We've never retrieved the full version of this emoticon, so we'll do that now
			// TODO: If time-limited caching is implemented, this is where that happens
			// We'll also want to make sure to exclude emoticons where Id = 0, because we injected
			// those ourselves
			if (emoticon.LastCacheUpdate == 0)
			{
				return await GetSingleEmoticonAndUpdateCacheAsync(shortcut, team);
			}

			switch (cacheBehavior)
			{
				case HipChatCacheBehavior.LoadFromCache:
					return emoticon;
				case HipChatCacheBehavior.RefreshCache:
					return await GetSingleEmoticonAndUpdateCacheAsync(shortcut, team);
				default:
					throw new ArgumentOutOfRangeException($"Unknown Cache Behavior - {cacheBehavior}", nameof(cacheBehavior));
			}
		}

		public async Task<IReadOnlyList<Emoticon>> GetEmoticonsForTeamAsync(Team team, HipChatCacheBehavior cacheBehavior = HipChatCacheBehavior.LoadFromCache)
		{
			switch (cacheBehavior)
			{
				case HipChatCacheBehavior.LoadFromCache:
					IReadOnlyList<Emoticon> emoticons = await _dataService.LoadEmoticonsForTeamAsync(team);
					if (!emoticons.Any())
					{
						// emoticon collection data wasn't cached, so we'll fetch it
						return await GetEmoticonsAndSaveToCacheAsync(team);
					}
					return emoticons;
				case HipChatCacheBehavior.RefreshCache:
					return await GetEmoticonsAndSaveToCacheAsync(team);
				default:
					throw new ArgumentOutOfRangeException($"Unknown Cache Behavior - {cacheBehavior}", nameof(cacheBehavior));
			}
		}

		private async Task<Emoticon> GetSingleEmoticonAndUpdateCacheAsync(string shortcut, Team team)
		{
			Emoticon emoticon = await GetSingleEmoticonFromServerAsync(shortcut, team);
			await _dataService.UpdateSingleEmoticonAsync(emoticon, team);
			return emoticon;
		}

		private async Task<IReadOnlyList<Emoticon>> GetEmoticonsAndSaveToCacheAsync(Team team)
		{
			IReadOnlyList<Emoticon> emoticons = await GetEmoticonsForTeamFromServerAsync(team);
			await _dataService.SaveEmoticonsForTeamAsync(emoticons, team);
			return emoticons;
		}

		private async Task<Emoticon> GetSingleEmoticonFromServerAsync(string shortcut, Team team)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);

			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(RootUri, $"/v2/emoticon/{shortcut}"));
			string json = await get.Content.ReadAsStringAsync();
			var hcEmoticon = JsonConvert.DeserializeObject<HipChatEmoticon>(json);

			return new Emoticon
			{
				Id = hcEmoticon.Id,
				Shortcut = hcEmoticon.Shortcut,
				Url = hcEmoticon.Url,
				Height = hcEmoticon.Height,
				Width = hcEmoticon.Width,
				LastCacheUpdate = DateTime.UtcNow.Ticks
			};
		}

		private async Task<IReadOnlyList<Emoticon>> GetEmoticonsForTeamFromServerAsync(Team team)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);

			var emoticons = new List<Emoticon>();
			var loadAnotherPage = true;

			while (loadAnotherPage)
			{
				HttpResponseMessage response = await GetPageOfEmoticons(_httpClient, emoticons.Count);
				string json = await response.Content.ReadAsStringAsync();
				var emoticonWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatEmoticonSummary>>(json);

				emoticons.AddRange(emoticonWrapper.Items.Select(hcEmoticon => new Emoticon
				{
					Id = hcEmoticon.Id,
					Shortcut = hcEmoticon.Shortcut,
					Url = hcEmoticon.Url,
					Height = PrecachedEmoticonDimensions,
					Width = PrecachedEmoticonDimensions,
					LastCacheUpdate = 0
				}));

				loadAnotherPage = emoticonWrapper.Items.Count() == MaxEmoticonResults;
			}

			emoticons.AddRange(GetMissingEmoticons(team));

			return emoticons;
		}

		private static async Task<HttpResponseMessage> GetPageOfEmoticons(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/emoticon?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxEmoticonResults}&"
						   + $"type={FilterEmoticonType}";

			return await httpClient.GetAsync(new Uri(RootUri, route));
		}

		/// <summary>
		/// There are some emoticons that the HipChat client renders that are not part of the emoticon system.
		/// This method returns a list containing those emoticons.
		/// </summary>
		/// <returns>A list of the client-only emoticons</returns>
		private static IEnumerable<Emoticon> GetMissingEmoticons(Team team)
		{
			IList<Emoticon> emoticons = new List<Emoticon>();

			emoticons.Add(BuildMissingEmoticon(team, "embarrassed", new Uri("ms-appx:///Assets/Emoji/flushed_face.png")));
			emoticons.Add(BuildMissingEmoticon(team, "oops", new Uri("ms-appx:///Assets/Emoji/disappointed_but_relieved_face.png")));
			emoticons.Add(BuildMissingEmoticon(team, "thumbsup", new Uri("ms-appx:///Assets/Emoji/thumbs_up.png")));
			emoticons.Add(BuildMissingEmoticon(team, "thumbsdown", new Uri("ms-appx:///Assets/Emoji/thumbs_down.png")));

			return emoticons;
		}

		private static Emoticon BuildMissingEmoticon(Team team, string shortcut, Uri uri)
		{
			return new Emoticon
			{
				Id = 0,
				Height = 18,
				Width = 18,
				LastCacheUpdate = DateTime.UtcNow.Ticks,
				Shortcut = shortcut,
				Team = team,
				Url = uri
			};
		}
	}
}