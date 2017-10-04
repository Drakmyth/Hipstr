using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
    public interface ISubscriptionService
    {
        Task AddSubscriptionAsync(IMessageSource messageSource);
        Task RemoveSubscriptionAsync(IMessageSource messageSource);
        Task<IReadOnlyList<IMessageSource>> GetSubscriptionsAsync(IHipChatService hipChatService);
        Task RemoveAllSubscriptionsForTeam(Team team);
    }
}