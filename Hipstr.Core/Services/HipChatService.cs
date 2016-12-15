using Hipstr.Core.Converters;
using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
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
	public class HipChatService : IHipChatService
	{
		// TODO: Notify user when API_KEY is about to expire
		private const int MaxRoomResults = 1000;
		private const bool IncludePrivateRooms = true;
		private const bool IncludeArchivedRooms = true;

		private static readonly Uri RootUri = new Uri("http://www.hipchat.com");

		private readonly ITeamService _teamService;
		private readonly IUserConverter _userConverter;

		public HipChatService(ITeamService teamService, IUserConverter userConverter)
		{
			_teamService = teamService;
			_userConverter = userConverter;
		}

		public async Task<IEnumerable<Room>> GetRoomsAsync()
		{
			IEnumerable<Team> teams = await _teamService.GetTeamsAsync();

			var taskTeamMapping = new Dictionary<Task<IEnumerable<Room>>, Team>();
			foreach (Team team in teams)
			{
				Task<IEnumerable<Room>> get = GetRoomsForTeam(team);
				taskTeamMapping.Add(get, team);
			}

			// TODO: Parallel processing of teams
			await Task.WhenAll(taskTeamMapping.Keys);

			var rooms = new List<Room>();
			foreach (Task<IEnumerable<Room>> task in taskTeamMapping.Keys)
			{
				rooms.AddRange(task.Result);
			}

			return rooms;
		}

		private async Task<IEnumerable<Room>> GetRoomsForTeam(Team team)
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);

			var rooms = new List<Room>();
			var loadAnotherPage = true;

			while (loadAnotherPage)
			{
				HttpResponseMessage response = await GetPageOfRooms(httpClient, rooms.Count);
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

		private async Task<HttpResponseMessage> GetPageOfRooms(HttpClient httpClient, int startIndex)
		{
			string route = "/v2/room?"
			               + $"start-index={startIndex}&"
			               + $"max-results={MaxRoomResults}&"
			               + $"include-private={IncludePrivateRooms}&"
			               + $"include-archived={IncludeArchivedRooms}";

			return await httpClient.GetAsync(new Uri(RootUri, route));
		}

		public async Task<IEnumerable<User>> GetUsersAsync()
		{
			var httpClient = new HttpClient();
			IEnumerable<Team> teams = await _teamService.GetTeamsAsync();

			var taskTeamMapping = new Dictionary<Task<HttpResponseMessage>, Team>();
			foreach (Team team in teams)
			{
				httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = httpClient.GetAsync(new Uri(RootUri, "/v2/user"));
				taskTeamMapping.Add(get, team);
			}

			// TODO: Parallel processing of teams
			await Task.WhenAll(taskTeamMapping.Keys);

			var users = new List<User>();
			foreach (Task<HttpResponseMessage> task in taskTeamMapping.Keys)
			{
				HttpResponseMessage response = task.Result;

				// TODO: Parallel processing of users
				string json = await response.Content.ReadAsStringAsync();
				var userWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatUser>>(json);
				users.AddRange(userWrapper.Items.Select(hcUser => _userConverter.HipChatUserToUser(hcUser, taskTeamMapping[task])));
			}

			return users;
		}

		public async Task<IEnumerable<Message>> GetMessagesAsync(Room room)
		{
			var httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			HttpResponseMessage get = await httpClient.GetAsync(new Uri(RootUri, $"/v2/room/{room.Id}/history"));
			string json = await get.Content.ReadAsStringAsync();

			var messageWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<object>>(json);
			IEnumerable<JObject> jsonObjects = messageWrapper.Items.Cast<JObject>().Where(jobj => jobj.Value<string>("type") == "message").ToList();
			var hcMessages = JsonConvert.DeserializeObject<IEnumerable<HipChatMessage>>(JsonConvert.SerializeObject(jsonObjects));

			return hcMessages.Select(hcMessage => new Message
			{
				PostedBy = _userConverter.HipChatUserToUser(hcMessage.From, room.Team),
				Date = hcMessage.Date,
				Text = hcMessage.Message
			}).ToList();
		}
	}
}