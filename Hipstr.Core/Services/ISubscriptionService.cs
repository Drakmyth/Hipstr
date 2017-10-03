using System.Collections.Generic;
using System.Threading.Tasks;
using Hipstr.Core.Messaging;

namespace Hipstr.Core.Services
{
	public interface ISubscriptionService
	{
		Task AddSubscriptionAsync(IMessageSource messageSource);
		Task RemoveSubscriptionAsync(IMessageSource messageSource);
		Task<IReadOnlyList<IMessageSource>> GetSubscriptionsAsync(IHipChatService hipChatService);
	}
}