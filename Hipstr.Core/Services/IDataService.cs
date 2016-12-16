using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IDataService
	{
		Task<IList<Team>> LoadTeamsAsync();
		Task<IList<RoomGroup>> LoadRoomGroupsAsync();
		Task<IList<User>> LoadUsersAsync();
		Task SaveTeamsAsync(IEnumerable<Team> teams);
		Task SaveRoomGroupsAsync(IEnumerable<RoomGroup> roomGroups);
		Task SaveUsersAsync(IEnumerable<User> users);
	}
}