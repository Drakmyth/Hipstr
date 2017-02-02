using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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

		private static readonly Uri _rootUri = new Uri("http://www.hipchat.com");

		private readonly IDataService _dataService;
		private readonly IHttpClient _httpClient;

		public HipChatService(IDataService dataService, IHttpClient httpClient)
		{
			_dataService = dataService;
			_httpClient = httpClient;
		}

		public async Task<Room> CreateRoomAsync(RoomCreationRequest request)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Team.ApiKey);

			HipChatRoomCreationRequest creationRequest = await BuildCreationRequest(request);
			HttpClientResponse<HipChatCreationResponse> creationResponse = await _httpClient.PostAsync<HipChatCreationResponse>(new Uri(_rootUri, "/v2/room"), creationRequest);
			HttpClientResponse<HipChatRoom> createdRoom = await _httpClient.GetAsync<HipChatRoom>(new Uri(_rootUri, $"/v2/room/{creationResponse.Payload.Id}"));
			return new Room
			{
				Id = createdRoom.Payload.Id,
				IsArchived = createdRoom.Payload.IsArchived,
				Name = createdRoom.Payload.Name,
				Privacy = createdRoom.Payload.Privacy,
				Team = request.Team
			};
		}

		private async Task<HipChatRoomCreationRequest> BuildCreationRequest(RoomCreationRequest request)
		{
			ApiKeyInfo keyInfo = await GetApiKeyInfoAsync(request.Team.ApiKey);

			return new HipChatRoomCreationRequest
			{
				DelegateAdminVisibility = request.DelegateAdminVisibility,
				GuestAccess = request.GuestAccess,
				Name = request.Name,
				OwnerUserId = keyInfo.Owner.Id.ToString(),
				Privacy = request.Privacy,
				Topic = request.Topic
			};
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
				HipChatCollectionWrapper<HipChatRoomSummary> roomWrapper = await GetPageOfRooms(_httpClient, rooms.Count);
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

		private static async Task<HipChatCollectionWrapper<HipChatRoomSummary>> GetPageOfRooms(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/room?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxRoomResults}&"
						   + $"include-private={IncludePrivateRooms}&"
						   + $"include-archived={IncludeArchivedRooms}";

			HttpClientResponse<HipChatCollectionWrapper<HipChatRoomSummary>> response = await httpClient.GetAsync<HipChatCollectionWrapper<HipChatRoomSummary>>(new Uri(_rootUri, route));
			return response.Payload;
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
				HipChatCollectionWrapper<HipChatUser> userWrapper = await GetPageOfUsers(_httpClient, users.Count);
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

		private static async Task<HipChatCollectionWrapper<HipChatUser>> GetPageOfUsers(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/user?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxUserResults}&"
						   + $"include-guests={IncludeGuestUsers}&"
						   + $"include-deleted={IncludeDeletedUsers}";

			HttpClientResponse<HipChatCollectionWrapper<HipChatUser>> response = await httpClient.GetAsync<HipChatCollectionWrapper<HipChatUser>>(new Uri(_rootUri, route));
			return response.Payload;
		}

		public async Task SendMessageToRoomAsync(Room room, string message)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			var payload = new HipChatMessageToRoom
			{
				Message = message
			};
			await _httpClient.PostAsync<object>(new Uri(_rootUri, $"/v2/room/{room.Id}/message"), payload);
		}

		public async Task<IReadOnlyList<Message>> GetMessagesForRoomAsync(Room room)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			HttpClientResponse<HipChatCollectionWrapper<object>> response = await _httpClient.GetAsync<HipChatCollectionWrapper<object>>(new Uri(_rootUri, $"/v2/room/{room.Id}/history"));
			return UnwrapMessages(room.Team, response.Payload);
		}

		private static IReadOnlyList<Message> UnwrapMessages(Team team, HipChatCollectionWrapper<object> wrapper)
		{
			IEnumerable<JObject> jObjects = wrapper.Items.Cast<JObject>().ToList();

			IList<Message> notifications = jObjects
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageTypes.Notification)
				.Select(jobj => jobj.ToObject<HipChatNotificationMessage>())
				.Select(hcNotification => BuildMessage(team, hcNotification))
				.ToList();

			IList<Message> messages = jObjects
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageTypes.Message) // TODO: Deserialize each message individually based on type
				.Select(jobj => jobj.ToObject<HipChatMessage>())
				.Select(hcMessage => BuildMessage(team, hcMessage))
				.ToList();

			return ParseMessages(messages.Concat(notifications).OrderBy(m => m.Date).ToList());
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

			if (hcMessage.MessageLinks != null)
			{
				messageBuilder = ParseMessageLinks(messageBuilder, hcMessage.MessageLinks.Cast<JObject>().ToList());
			}

			if (hcMessage.File != null)
			{
				messageBuilder.WithFile(new MessageFile
				{
					FileSize = hcMessage.File.FileSize,
					Name = hcMessage.File.Name,
					ThumbnailUri = hcMessage.File.ThumbnailUri,
					Uri = hcMessage.File.Uri
				});
			}

			return messageBuilder.Build();
		}

		private static IMessageBuilder ParseMessageLinks(IMessageBuilder builder, IList<JObject> messageLinks)
		{
			IEnumerable<MessageImage> images = messageLinks
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Image)
				.Select(jobj => jobj.Value<JObject>("image").ToObject<HipChatMessageImage>())
				.Select(hcMessageImage => new MessageImage
				{
					ImageUri = hcMessageImage.ImageUri,
					Name = hcMessageImage.Name
				})
				.ToList();
			IEnumerable<MessageLink> links = messageLinks
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Link)
				.Select(jobj => jobj.Value<JObject>("link").ToObject<HipChatMessageLink>())
				.Select(hcMessageLink => new MessageLink
				{
					Description = hcMessageLink.Description,
					FaviconUri = hcMessageLink.FaviconUri,
					FullUri = hcMessageLink.FullUri,
					HeaderText = hcMessageLink.HeaderText,
					LinkText = hcMessageLink.LinkText,
					Title = hcMessageLink.Title
				})
				.ToList();
			IEnumerable<MessageTwitterUser> twitterUsers = messageLinks
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.TwitterUser)
				.Select(jobj => jobj.Value<JObject>("twitter_user").ToObject<HipChatMessageTwitterUser>())
				.Select(hcMessageTwitterUser => new MessageTwitterUser
				{
					Followers = hcMessageTwitterUser.Followers,
					Name = hcMessageTwitterUser.Name,
					ProfileImageUri = hcMessageTwitterUser.ProfileImageUri,
					ScreenName = hcMessageTwitterUser.ScreenName
				})
				.ToList();
			IEnumerable<MessageTwitterStatus> twitterStatuses = messageLinks
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.TwitterStatus)
				.Select(jobj => jobj.Value<JObject>("twitter_status").ToObject<HipChatMessageTwitterStatus>())
				.Select(hcMessageTwitterStatus => new MessageTwitterStatus
				{
					Created = hcMessageTwitterStatus.Created,
					Name = hcMessageTwitterStatus.Name,
					ProfileImageUri = hcMessageTwitterStatus.ProfileImageUri,
					ScreenName = hcMessageTwitterStatus.ScreenName,
					Source = hcMessageTwitterStatus.Source,
					Text = hcMessageTwitterStatus.Text
				})
				.ToList();
			IEnumerable<MessageVideo> videos = messageLinks
				.Where(jobj => jobj.Value<string>("type") == HipChatMessageLinkTypes.Video)
				.Select(jobj => jobj.Value<JObject>("video").ToObject<HipChatMessageVideo>())
				.Select(hcMessageVideo => new MessageVideo
				{
					Author = hcMessageVideo.Author,
					ThumbnailUri = hcMessageVideo.ThumbnailUri,
					Title = hcMessageVideo.Title,
					Views = hcMessageVideo.Views
				})
				.ToList();

			return builder.WithImages(images)
				.WithLinks(links)
				.WithTwitterUsers(twitterUsers)
				.WithTwitterStatuses(twitterStatuses)
				.WithVideos(videos);
		}

		private static Message BuildMessage(Team team, HipChatNotificationMessage hcMessage)
		{
			IMessageBuilder messageBuilder = Message.Builder(
				new User
				{
					Id = 0,
					Handle = "",
					Name = hcMessage.From,
					Team = team
				},
				hcMessage.Date,
				hcMessage.Message);

			return hcMessage.MessageLinks == null
				? messageBuilder.Build()
				: ParseMessageLinks(messageBuilder, hcMessage.MessageLinks.Cast<JObject>().ToList()).Build();
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
			await _httpClient.PostAsync<object>(new Uri(_rootUri, $"/v2/user/{user.Id}/message"), payload);
		}

		public async Task<IReadOnlyList<Message>> GetMessagesForUserAsync(User user)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.Team.ApiKey);
			HttpClientResponse<HipChatCollectionWrapper<object>> response = await _httpClient.GetAsync<HipChatCollectionWrapper<object>>(new Uri(_rootUri, $"/v2/user/{user.Id}/history"));
			return UnwrapMessages(user.Team, response.Payload);
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

			HttpClientResponse<HipChatUserProfile> response = await _httpClient.GetAsync<HipChatUserProfile>(new Uri(_rootUri, $"/v2/user/{user.Id}"));
			var hcUserProfile = response.Payload;

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

			HttpClientResponse<HipChatOAuthSession> response = await _httpClient.GetAsync<HipChatOAuthSession>(new Uri(_rootUri, $"/v2/oauth/token/{apiKey}"));
			HipChatOAuthSession hcSession = response.Payload;

			return new ApiKeyInfo
			{
				Id = hcSession.Id,
				ApiKey = hcSession.AccessToken,
				RefreshToken = hcSession.RefreshToken,
				Scopes = hcSession.Scopes,
				Owner = new User
				{
					Handle = hcSession.Owner.MentionName,
					Id = hcSession.Owner.Id,
					Name = hcSession.Owner.Name,
					Team = new Team("", apiKey) // TODO: Figure out a better way to build this object so the name isn't missing
				}
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

			HttpClientResponse<HipChatEmoticon> response = await _httpClient.GetAsync<HipChatEmoticon>(new Uri(_rootUri, $"/v2/emoticon/{shortcut}"));
			HipChatEmoticon hcEmoticon = response.Payload;

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
				HipChatCollectionWrapper<HipChatEmoticonSummary> emoticonWrapper = await GetPageOfEmoticons(_httpClient, emoticons.Count);
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

		private static async Task<HipChatCollectionWrapper<HipChatEmoticonSummary>> GetPageOfEmoticons(IHttpClient httpClient, int startIndex)
		{
			string route = "/v2/emoticon?"
						   + $"start-index={startIndex}&"
						   + $"max-results={MaxEmoticonResults}&"
						   + $"type={FilterEmoticonType}";

			var response = await httpClient.GetAsync<HipChatCollectionWrapper<HipChatEmoticonSummary>>(new Uri(_rootUri, route));
			return response.Payload;
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