using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Core.Services
{
	public class TeamService : ITeamService
	{
		private readonly List<Team> _teams;

		public TeamService()
		{
			_teams = new List<Team>();
		}

		public void AddTeam(Team team)
		{
			_teams.Add(team);
		}

		public bool TeamExists(string apiKey)
		{
			return _teams.Any(t => t.ApiKey == apiKey);
		}

		public void RemoveTeam(Team team)
		{
			_teams.Remove(team);
		}

		public IEnumerable<Team> GetTeams()
		{
			return _teams;
		}
	}
}