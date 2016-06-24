using System.Collections.Generic;
using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
	public interface ITeamService
	{
		void AddTeam(Team team);
		bool TeamExists(string apiKey);
		void RemoveTeam(Team team);
		IEnumerable<Team> GetTeams();
	}
}