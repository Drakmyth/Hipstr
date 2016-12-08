using Hipstr.Core.Models;
using Hipstr.Core.Utility.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public class TeamService : ITeamService
	{
		private readonly IList<Team> _teams;
		private readonly IDataService _dataService;

		public TeamService(IDataService dataService)
		{
			_teams = new List<Team>();
			_dataService = dataService;
		}

		public async void AddTeamAsync(Team team)
		{
			_teams.Add(team);
			await _dataService.SaveTeamsAsync(_teams);
		}

		public bool TeamExists(string apiKey)
		{
			return _teams.Any(t => t.ApiKey == apiKey);
		}

		public async Task RemoveTeamAsync(Team team)
		{
			_teams.Remove(team);
			await _dataService.SaveTeamsAsync(_teams);
		}

		public async Task<IEnumerable<Team>> GetTeamsAsync()
		{
			_teams.Clear();
			_teams.AddRange(await _dataService.LoadTeamsAsync());
			return new ReadOnlyCollection<Team>(_teams);
		}
	}
}