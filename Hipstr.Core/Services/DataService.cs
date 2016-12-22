using Hipstr.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace Hipstr.Core.Services
{
	public class DataService : IDataService
	{
		private const string TeamsFileName = "teams.json";
		private const string RoomGroupsFileName = "roomGroups.json";
		private const string UserGroupsFileName = "userGroups.json";

		public async Task<IList<Team>> LoadTeamsAsync()
		{
			return await LoadDataAsync<Team>(TeamsFileName);
		}

		public async Task<IList<RoomGroup>> LoadRoomGroupsAsync()
		{
			try
			{
				return await LoadDataAsync<RoomGroup>(RoomGroupsFileName);
			}
			catch (JsonSerializationException)
			{
				return new List<RoomGroup>();
			}
		}

		public async Task<IList<UserGroup>> LoadUserGroupsAsync()
		{
			return await LoadDataAsync<UserGroup>(UserGroupsFileName);
		}

		public async Task SaveTeamsAsync(IEnumerable<Team> teams)
		{
			await SaveDataAsync(TeamsFileName, teams);
		}

		public async Task SaveRoomGroupsAsync(IEnumerable<RoomGroup> roomGroups)
		{
			await SaveDataAsync(RoomGroupsFileName, roomGroups);
		}

		public async Task SaveUserGroupsAsync(IEnumerable<UserGroup> userGroups)
		{
			await SaveDataAsync(UserGroupsFileName, userGroups);
		}

		private async Task<IList<T>> LoadDataAsync<T>(string filename)
		{
			StorageFile file = await GetStorageFileAsync(filename);
			string json = await FileIO.ReadTextAsync(file);
			return string.IsNullOrEmpty(json) ? new List<T>() : JsonConvert.DeserializeObject<IList<T>>(json);
		}

		private async Task SaveDataAsync<T>(string filename, IEnumerable<T> data)
		{
			StorageFile file = await GetStorageFileAsync(filename);
			string json = JsonConvert.SerializeObject(data);
			await FileIO.WriteTextAsync(file, json);
		}

		private async Task<StorageFile> GetStorageFileAsync(string filename)
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			return await folder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
		}
	}
}