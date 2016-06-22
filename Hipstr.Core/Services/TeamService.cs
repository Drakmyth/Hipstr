using System.Collections.Generic;
using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
	public class TeamService : ITeamService
	{
		private List<Team> _teams;

		public TeamService()
		{
			_teams = new List<Team>();
		}

		public void AddTeam(Team team)
		{
			_teams.Add(team);
		}

		public void RemoveTeam(Team team)
		{
			_teams.Remove(team);
		}
	}
}