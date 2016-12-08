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

		private readonly Uri ROOT_URI = new Uri("http://www.hipchat.com");

		private readonly ITeamService _teamService;
		private readonly HttpClient _httpClient;
		private readonly IUserConverter _userConverter;

		public HipChatService(HttpClient httpClient, ITeamService teamService, IUserConverter userConverter)
		{
			_httpClient = httpClient;
			_teamService = teamService;
			_userConverter = userConverter;
		}

		public async Task<IEnumerable<Room>> GetRoomsAsync()
		{
			IEnumerable<Team> teams = await _teamService.GetTeamsAsync();

			var taskTeamMapping = new Dictionary<Task<HttpResponseMessage>, Team>();
			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/room"));
				taskTeamMapping.Add(get, team);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(taskTeamMapping.Keys);

			// TODO: Don't wait for all teams to return room list before processing results
			requestTasks.Wait();

			var rooms = new List<Room>();
			foreach (Task<HttpResponseMessage> task in taskTeamMapping.Keys)
			{
				HttpResponseMessage response = task.Result;
				Task<string> json = response.Content.ReadAsStringAsync();

				// TODO: Process rooms in parallel
				json.Wait();
				var roomWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatRoom>>(json.Result);
				rooms.AddRange(roomWrapper.Items.Select(hcRoom => new Room
				{
					Id = hcRoom.Id,
					IsArchived = hcRoom.IsArchived,
					Name = hcRoom.Name,
					Privacy = hcRoom.Privacy,
					Team = taskTeamMapping[task]
				}));
			}

			return rooms;
		}

		public async Task<IEnumerable<User>> GetUsersAsync()
		{
			IEnumerable<Team> teams = await _teamService.GetTeamsAsync();

			var taskTeamMapping = new Dictionary<Task<HttpResponseMessage>, Team>();
			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/user"));
				taskTeamMapping.Add(get, team);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(taskTeamMapping.Keys);

			// TODO: Don't wait for all teams to return user list before processing results
			requestTasks.Wait();

			var users = new List<User>();
			foreach (Task<HttpResponseMessage> task in taskTeamMapping.Keys)
			{
				HttpResponseMessage response = task.Result;
				Task<string> json = response.Content.ReadAsStringAsync();

				// TODO: Process users in parallel
				json.Wait();
				var userWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatUser>>(json.Result);
				users.AddRange(userWrapper.Items.Select(hcUser => _userConverter.HipChatUserToUser(hcUser, taskTeamMapping[task])));
			}

			return users;
		}

		public async Task<IEnumerable<Message>> GetMessagesAsync(Room room)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			HttpResponseMessage get = await _httpClient.GetAsync(new Uri(ROOT_URI, $"/v2/room/{room.Id}/history"));
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