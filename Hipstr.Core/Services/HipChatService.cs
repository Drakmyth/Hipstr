using Hipstr.Core.Models.HipChat;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public class HipChatService : IHipChatService
	{
		// TODO: Get API_KEY from user
		// TODO: Notify user when API_KEY is about to expire
		private const string API_KEY = "---API KEY GOES HERE---"; // API_KEY is good for 1 year from generation date.
		private readonly Uri ROOT_URI = new Uri("http://www.hipchat.com");

		private readonly HttpClient _httpClient;

		public HipChatService(HttpClient httpClient)
		{
			_httpClient = httpClient;
			_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", API_KEY);
		}

		public CollectionWrapper<RoomSummary> GetRooms()
		{
			Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/room"));
			get.Wait();

			Task<string> json = get.Result.Content.ReadAsStringAsync();
			json.Wait();

			CollectionWrapper<RoomSummary> summary = JsonConvert.DeserializeObject<CollectionWrapper<RoomSummary>>(json.Result);
			return summary;
		}

		public CollectionWrapper<UserSummary> GetUsers()
		{
			Task<HttpResponseMessage> get = _httpClient.GetAsync(new Uri(ROOT_URI, "/v2/user"));
			get.Wait();

			Task<string> json = get.Result.Content.ReadAsStringAsync();
			json.Wait();

			CollectionWrapper<UserSummary> summary = JsonConvert.DeserializeObject<CollectionWrapper<UserSummary>>(json.Result);
			return summary;
		}
	}
}