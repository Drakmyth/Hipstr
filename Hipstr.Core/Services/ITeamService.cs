using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
	public interface ITeamService
	{
		void AddTeam(Team team);
		void RemoveTeam(Team team);
	}
}