using Hipstr.Core.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Hipstr.Core.Services
{
	public class TeamService : ITeamService
	{
		private List<Team> _teams;

		public TeamService()
		{
			_teams = new List<Team>();
		}

		public async void AddTeamAsync(Team team)
		{
			_teams.Add(team);

			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await folder.CreateFileAsync("myFile.txt", CreationCollisionOption.OpenIfExists);
			string json = JsonConvert.SerializeObject(_teams);
			await FileIO.WriteTextAsync(storageFile, json);
		}

		public bool TeamExists(string apiKey)
		{
			return _teams.Any(t => t.ApiKey == apiKey);
		}

		public void RemoveTeam(Team team)
		{
			_teams.Remove(team);
		}

		public async Task<IEnumerable<Team>> GetTeamsAsync()
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await folder.CreateFileAsync("myFile.txt", CreationCollisionOption.OpenIfExists);
			string json = await FileIO.ReadTextAsync(storageFile);
			_teams = string.IsNullOrEmpty(json) ? new List<Team>() : JsonConvert.DeserializeObject<List<Team>>(json);

			return _teams;
		}
	}
}