using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface ITeamService
	{
		Task AddTeamAsync(Team team);
		Task EditTeamAsync(string oldApiKey, Team team);
		bool TeamExists(string apiKey);
		Task RemoveTeamAsync(Team team);
		Task<IEnumerable<Team>> GetTeamsAsync();
	}
}