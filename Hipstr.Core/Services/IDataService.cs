using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IDataService
	{
		Task SaveTeamsAsync(IEnumerable<Team> teams);
		Task<IReadOnlyList<Team>> LoadTeamsAsync();
		Task SaveRoomsForTeamAsync(IEnumerable<Room> rooms, Team team);
		Task<IReadOnlyList<Room>> LoadRoomsForTeamAsync(Team team);
		Task SaveUsersForTeamAsync(IEnumerable<User> users, Team team);
		Task<IReadOnlyList<User>> LoadUsersForTeamAsync(Team team);
		Task SaveEmoticonsForTeamAsync(IEnumerable<Emoticon> emoticons, Team team);
		Task<Emoticon> UpdateSingleEmoticonAsync(Emoticon emoticon, Team team);
		Task<IReadOnlyList<Emoticon>> LoadEmoticonsForTeamAsync(Team team);
	}
}