using Hipstr.Core.Messaging;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hipstr.Client.Views.Subscriptions
{
	public class MessageSourceGroup : IGrouping<string, IMessageSource>
	{
		private readonly List<IMessageSource> _messageSources;
		public string Key { get; }

		public MessageSourceGroup(string key, IEnumerable<IMessageSource> messageSources)
		{
			Key = key;
			_messageSources = new List<IMessageSource>(messageSources);
		}

		public IEnumerator<IMessageSource> GetEnumerator() => _messageSources.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _messageSources.GetEnumerator();
	}

	[UsedImplicitly]
	public class SubscriptionsViewModel : ViewModelBase
	{
		public ObservableCollection<MessageSourceGroup> GroupedSubscriptions { get; }

		private readonly IHipChatService _hipChatService;
		private readonly ISubscriptionService _subscriptionService;

		public SubscriptionsViewModel(IHipChatService hipChatService, ISubscriptionService subscriptionService)
		{
			_hipChatService = hipChatService;
			_subscriptionService = subscriptionService;

			GroupedSubscriptions = new ObservableCollection<MessageSourceGroup>();
		}

		public override async Task InitializeAsync()
		{
			if (GroupedSubscriptions.Any()) return;

			IReadOnlyList<IMessageSource> subscriptions = await _subscriptionService.GetSubscriptionsAsync(_hipChatService);

			IEnumerable<MessageSourceGroup> groupedSources =
				subscriptions.GroupBy(
					GroupNameSelector,
					(key, sources) => new MessageSourceGroup(key, sources.OrderBy(ums => ums.Name)));
			GroupedSubscriptions.Clear();
			GroupedSubscriptions.AddRange(groupedSources.OrderBy(msg => msg.Key));
		}

		private static string GroupNameSelector(IMessageSource s)
		{
			string name = s.Name.Normalize(NormalizationForm.FormD);
			char key = name.FirstOrDefault(char.IsLetterOrDigit);

			if (!char.IsLetterOrDigit(key))
			{
				return "\U0001F310";
			}

			return char.IsLetter(key) ? char.ToUpper(key).ToString() : "#";
		}
	}
}