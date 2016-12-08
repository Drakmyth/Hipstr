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

		public async Task<IList<Team>> LoadTeamsAsync()
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await folder.CreateFileAsync(TeamsFileName, CreationCollisionOption.OpenIfExists);
			string json = await FileIO.ReadTextAsync(storageFile);
			return string.IsNullOrEmpty(json) ? new List<Team>() : JsonConvert.DeserializeObject<IList<Team>>(json);
		}

		public async Task SaveTeamsAsync(IEnumerable<Team> teams)
		{
			StorageFolder folder = ApplicationData.Current.LocalFolder;
			StorageFile storageFile = await folder.CreateFileAsync(TeamsFileName, CreationCollisionOption.OpenIfExists);
			string json = JsonConvert.SerializeObject(teams);
			await FileIO.WriteTextAsync(storageFile, json);
		}
	}
}