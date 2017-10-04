using Hipstr.Core.Models;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
    [UsedImplicitly]
    public class TeamService : ITeamService
    {
        private readonly IList<Team> _teams;
        private readonly IDataService _dataService;
        private readonly ISubscriptionService _subscriptionService;

        public TeamService(IDataService dataService, ISubscriptionService subscriptionService)
        {
            _teams = new List<Team>();
            _dataService = dataService;
            _subscriptionService = subscriptionService;
        }

        public async Task AddTeamAsync(Team team)
        {
            _teams.Add(team);
            await _dataService.SaveTeamsAsync(_teams);
        }

        public async Task EditTeamAsync(Team team)
        {
            Team teamToEdit = _teams.Where(t => t.ApiKey == team.ApiKey).Single();
            teamToEdit.Name = team.Name;
            await _dataService.SaveTeamsAsync(_teams);
        }

        public async Task RemoveTeamAsync(Team team)
        {
            _teams.Remove(team);
            await _subscriptionService.RemoveAllSubscriptionsForTeam(team);
            await _dataService.SaveTeamsAsync(_teams);
        }

        public async Task<IReadOnlyList<Team>> GetTeamsAsync()
        {
            _teams.Clear();
            _teams.AddRange(await _dataService.LoadTeamsAsync());
            return new ReadOnlyCollection<Team>(_teams);
        }
    }
}