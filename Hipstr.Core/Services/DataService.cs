﻿using Hipstr.Core.Models;
using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
			return await LoadCollectionDataAsync<Team>("teams.json");
		}

		public async Task SaveRoomsForTeamAsync(IEnumerable<Room> rooms, Team team)
		{
			await SaveDataAsync($"Rooms-{team.ApiKey}.json", rooms);
		}

		public async Task<IReadOnlyList<Room>> LoadRoomsForTeamAsync(Team team)
		{
			IReadOnlyList<Room> rooms = await LoadCollectionDataAsync<Room>($"Rooms-{team.ApiKey}.json");
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
			IReadOnlyList<User> users = await LoadCollectionDataAsync<User>($"Users-{team.ApiKey}.json");
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
			IReadOnlyList<Emoticon> emoticons = await LoadCollectionDataAsync<Emoticon>($"Emoticons-{team.ApiKey}.json");
			foreach (Emoticon emoticon in emoticons)
			{
				emoticon.Team = team;
			}

			return emoticons;
		}

		public async Task<Emoticon> UpdateSingleEmoticonAsync(Emoticon emoticon, Team team)
		{
			IReadOnlyList<Emoticon> emoticons = await LoadCollectionDataAsync<Emoticon>($"Emoticons-{team.ApiKey}.json");
			Emoticon toUpdate = emoticons.Where(e => e.Id == emoticon.Id).Single();
			toUpdate.Height = emoticon.Height;
			toUpdate.Width = emoticon.Width;
			toUpdate.LastCacheUpdate = emoticon.LastCacheUpdate;
			await SaveDataAsync($"Emoticons-{team.ApiKey}.json", emoticons);
			return emoticon;
		}

		private static async Task SaveDataAsync<T>(string filename, T data)
		{
			StorageFile file = await GetStorageFileAsync(filename);
			string json = JsonConvert.SerializeObject(data);
			await FileIO.WriteTextAsync(file, json);
		}

		private static async Task<IReadOnlyList<T>> LoadCollectionDataAsync<T>(string filename)
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