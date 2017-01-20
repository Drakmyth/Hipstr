using Hipstr.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Hipstr.Core.Services
{
	[UsedImplicitly]
	public class DataService : IDataService
	{
		public async Task SaveTeamsAsync(IEnumerable<Team> teams)
		{
			await SaveDataAsync("teams.json", teams);
		}

		public async Task<IReadOnlyList<Team>> LoadTeamsAsync()
		{
			return await LoadDataAsync<Team>("teams.json");
		}

		public async Task SaveRoomsForTeamAsync(IEnumerable<Room> rooms, Team team)
		{
			await SaveDataAsync($"Rooms-{team.ApiKey}.json", rooms);
		}

		public async Task<IReadOnlyList<Room>> LoadRoomsForTeamAsync(Team team)
		{
			IReadOnlyList<Room> rooms = await LoadDataAsync<Room>($"Rooms-{team.ApiKey}.json");
			foreach (Room room in rooms)
			{
				room.Team = team;
			}

			return rooms;
		}

		public async Task SaveUsersForTeamAsync(IEnumerable<User> users, Team team)
		{
			await SaveDataAsync($"Users-{team.ApiKey}.json", users);
		}

		public async Task<IReadOnlyList<User>> LoadUsersForTeamAsync(Team team)
		{
			IReadOnlyList<User> users = await LoadDataAsync<User>($"Users-{team.ApiKey}.json");
			foreach (User user in users)
			{
				user.Team = team;
			}

			return users;
		}

		public async Task SaveEmoticonsForTeamAsync(IEnumerable<Emoticon> emoticons, Team team)
		{
			await SaveDataAsync($"Emoticons-{team.ApiKey}.json", emoticons);
		}

		public async Task<IReadOnlyList<Emoticon>> LoadEmoticonsForTeamAsync(Team team)
		{
			IReadOnlyList<Emoticon> emoticons = await LoadDataAsync<Emoticon>($"Emoticons-{team.ApiKey}.json");
			foreach (Emoticon emoticon in emoticons)
			{
				emoticon.Team = team;
			}

			return emoticons;
		}

		private static async Task SaveDataAsync<T>(string filename, IEnumerable<T> data)
		{
			StorageFile file = await GetStorageFileAsync(filename);
			string json = JsonConvert.SerializeObject(data);
			await FileIO.WriteTextAsync(file, json);
		}

		private static async Task<IReadOnlyList<T>> LoadDataAsync<T>(string filename)
		{
			StorageFile file = await GetStorageFileAsync(filename);
			string json = await FileIO.ReadTextAsync(file);
			return string.IsNullOrEmpty(json) ? new List<T>().AsReadOnly() : JsonConvert.DeserializeObject<IReadOnlyList<T>>(json);
		}

		private static async Task<StorageFile> GetStorageFileAsync(string filename)
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			return await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
		}
	}
}