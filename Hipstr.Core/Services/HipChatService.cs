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

		public HipChatService(HttpClient httpClient, ITeamService teamService)
		{
			_httpClient = httpClient;
			_teamService = teamService;
		}

		public IEnumerable<Room> GetRooms()
		{
			IEnumerable<Team> teams = _teamService.GetTeams();

			List<Task<HttpResponseMessage>> roomRequests = new List<Task<HttpResponseMessage>>();

			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/room"));
				roomRequests.Add(get);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(roomRequests);
			requestTasks.Wait();

			List<Room> rooms = new List<Room>();
			foreach (HttpResponseMessage response in requestTasks.Result)
			{
				Task<string> json = response.Content.ReadAsStringAsync();
				json.Wait();
				HipChatCollectionWrapper<HipChatRoom> roomWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatRoom>>(json.Result);
				foreach (HipChatRoom hcRoom in roomWrapper.Items)
				{
					Room room = new Room
					{
						Id = hcRoom.Id,
						IsArchived = hcRoom.IsArchived,
						Name = hcRoom.Name,
						Privacy = hcRoom.Privacy
						// Team = team
					};

					rooms.Add(room);
				}
			}

			return rooms;
		}

		public IEnumerable<User> GetUsers()
		{
			IEnumerable<Team> teams = _teamService.GetTeams();

			List<Task<HttpResponseMessage>> userRequests = new List<Task<HttpResponseMessage>>();

			foreach (Team team in teams)
			{
				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", team.ApiKey);
				Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/user"));
				userRequests.Add(get);
			}

			Task<HttpResponseMessage[]> requestTasks = Task.WhenAll(userRequests);
			requestTasks.Wait();

			List<User> users = new List<User>();
			foreach (HttpResponseMessage response in requestTasks.Result)
			{
				Task<string> json = response.Content.ReadAsStringAsync();
				json.Wait();
				HipChatCollectionWrapper<HipChatUser> userWrapper = JsonConvert.DeserializeObject<HipChatCollectionWrapper<HipChatUser>>(json.Result);
				foreach (HipChatUser hcUser in userWrapper.Items)
				{
					User user = new User
					{
						Id = hcUser.Id,
						Handle = hcUser.MentionName,
						Name = hcUser.Name
					};

					users.Add(user);
				}
			}

			return users;
		}
	}
}