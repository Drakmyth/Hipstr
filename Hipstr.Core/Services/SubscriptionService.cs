using Hipstr.Core.Messaging;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
    [UsedImplicitly]
    public class SubscriptionService : ISubscriptionService
    {
        private readonly IDataService _dataService;
        private readonly IList<IMessageSource> _subscriptions;

        public SubscriptionService(IDataService dataService)
        {
            _dataService = dataService;
            _subscriptions = new List<IMessageSource>();
        }

        public async Task AddSubscriptionAsync(IMessageSource messageSource)
        {
            _subscriptions.Add(messageSource);
            await _dataService.SaveSubscriptionsAsync(_subscriptions);
        }

        public async Task RemoveSubscriptionAsync(IMessageSource messageSource)
        {
            _subscriptions.Remove(messageSource);
            await _dataService.SaveSubscriptionsAsync(_subscriptions);
        }

        public async Task RemoveAllSubscriptionsForTeam(Team team)
        {
            List<IMessageSource> teamSubscriptions = _subscriptions.Where(s => s.Team.ApiKey == team.ApiKey).ToList();
            foreach (IMessageSource subscription in teamSubscriptions)
            {
                _subscriptions.Remove(subscription);
            }

            await _dataService.SaveSubscriptionsAsync(_subscriptions);
        }

        public async Task<IReadOnlyList<IMessageSource>> GetSubscriptionsAsync(IHipChatService hipChatService)
        {
            _subscriptions.Clear();
            _subscriptions.AddRange(await _dataService.LoadSubscriptionsAsync(hipChatService));
            return new ReadOnlyCollection<IMessageSource>(_subscriptions);
        }
    }
}