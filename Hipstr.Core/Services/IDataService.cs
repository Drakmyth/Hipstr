using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IDataService
	{
		Task<IList<Team>> LoadTeamsAsync();
		Task SaveTeamsAsync(IEnumerable<Team> teams);
	}
}