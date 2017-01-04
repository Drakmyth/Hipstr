using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface ITeamService
	{
		Task AddTeamAsync(Team team);
		Task EditTeamAsync(Team team);
		Task RemoveTeamAsync(Team team);
		Task<IReadOnlyList<Team>> GetTeamsAsync();
	}
}