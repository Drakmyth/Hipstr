using Hipstr.Core.Messaging;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	[UsedImplicitly]
	public class SubscriptionService : ISubscriptionService
	{
		private readonly IDataService _dataService;
		private readonly List<IMessageSource> _subscriptions;

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

		public async Task<IReadOnlyList<IMessageSource>> GetSubscriptionsAsync(IHipChatService hipChatService)
		{
			return await _dataService.LoadSubscriptionsAsync(hipChatService);
		}
	}
}