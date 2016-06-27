using Hipstr.Core.Converters;
using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public class HipChatService : IHipChatService
	{
		// TODO: Get API_KEY from user
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

		public IEnumerable<Room> GetRooms()
		{
			IEnumerable<Team> teams = _teamService.GetTeams();

			Dictionary<Task<HttpResponseMessage>, Team> taskTeamMapping = new Dictionary<Task<HttpResponseMessage>, Team>();
			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/room"));
				taskTeamMapping.Add(get, team);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(taskTeamMapping.Keys);

			// TODO: Don't wait for all teams to return room list before processing results
			requestTasks.Wait();

			List<Room> rooms = new List<Room>();
			foreach (Task<HttpResponseMessage> task in taskTeamMapping.Keys)
			{
				HttpResponseMessage response = task.Result;
				Task<string> json = response.Content.ReadAsStringAsync();

				// TODO: Process rooms in parallel
				json.Wait();
				HipChatCollectionWrapper<HipChatRoom> roomWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatRoom>>(json.Result);
				foreach (HipChatRoom hcRoom in roomWrapper.Items)
				{
					Room room = new Room
					{
						Id = hcRoom.Id,
						IsArchived = hcRoom.IsArchived,
						Name = hcRoom.Name,
						Privacy = hcRoom.Privacy,
						Team = taskTeamMapping[task]
					};

					rooms.Add(room);
				}
			}

			return rooms;
		}

		public IEnumerable<User> GetUsers()
		{
			IEnumerable<Team> teams = _teamService.GetTeams();

			Dictionary<Task<HttpResponseMessage>, Team> taskTeamMapping = new Dictionary<Task<HttpResponseMessage>, Team>();
			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/user"));
				taskTeamMapping.Add(get, team);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(taskTeamMapping.Keys);

			// TODO: Don't wait for all teams to return user list before processing results
			requestTasks.Wait();

			List<User> users = new List<User>();
			foreach (Task<HttpResponseMessage> task in taskTeamMapping.Keys)
			{
				HttpResponseMessage response = task.Result;
				Task<string> json = response.Content.ReadAsStringAsync();
				json.Wait();
				HipChatCollectionWrapper<HipChatUser> userWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatUser>>(json.Result);
				foreach (HipChatUser hcUser in userWrapper.Items)
				{
					User user = _userConverter.HipChatUserToUser(hcUser, taskTeamMapping[task]);
					users.Add(user);
				}
			}

			return users;
		}

		public IEnumerable<Message> GetMessages(Room room)
		{
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", room.Team.ApiKey);
			Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, $"/v2/room/{room.Id}/history"));
			get.Wait();

			Task<string> json = get.Result.Content.ReadAsStringAsync();
			json.Wait();

			List<Message> messages = new List<Message>();
			HipChatCollectionWrapper<HipChatMessage> messageWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatMessage>>(json.Result);
			foreach (HipChatMessage hcMessage in messageWrapper.Items)
			{
				Message message = new Message
				{
					PostedBy = _userConverter.HipChatUserToUser(hcMessage.From, room.Team),
					Date = hcMessage.Date,
					Text = hcMessage.Message
				};

				messages.Add(message);
			}

			return messages;
		}
	}
}