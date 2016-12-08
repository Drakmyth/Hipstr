﻿using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface ITeamService
	{
		void AddTeamAsync(Team team);
		bool TeamExists(string apiKey);
		void RemoveTeam(Team team);
		Task<IEnumerable<Team>> GetTeamsAsync();
	}
}