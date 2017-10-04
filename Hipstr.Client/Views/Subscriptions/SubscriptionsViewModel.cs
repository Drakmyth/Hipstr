using Hipstr.Client.Utility;
using Hipstr.Core.Messaging;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Hipstr.Client.Views.Subscriptions
{
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

		public override void Initialize()
		{
			base.Initialize();

			Eventing.RoomJoined += OnRoomJoined;
            Eventing.TeamDeleted += OnTeamDeleted;
		}

        public override void Dispose()
        {
            Eventing.RoomJoined -= OnRoomJoined;
            Eventing.TeamDeleted -= OnTeamDeleted;
        }

        private async void OnTeamDeleted(object sender, TeamDeletedEventArgs e)
        {
            await RefreshSubscriptionsAsync();
        }

        private async void OnRoomJoined(object sender, RoomJoinedEventArgs roomJoinedEventArgs)
		{
			await RefreshSubscriptionsAsync();
		}

		public override async Task InitializeAsync()
		{
			if (GroupedSubscriptions.Any()) return;

			await RefreshSubscriptionsAsync();
		}

		private async Task RefreshSubscriptionsAsync()
		{
			IReadOnlyList<IMessageSource> subscriptions = await _subscriptionService.GetSubscriptionsAsync(_hipChatService);

			IEnumerable<MessageSourceGroup> groupedSources =
				subscriptions.GroupBy(
					GroupNameSelectors.StandardGroupNameSelector,
					(key, sources) => new MessageSourceGroup(key, sources.OrderBy(ums => ums.Name)));
			GroupedSubscriptions.Clear();
			GroupedSubscriptions.AddRange(groupedSources.OrderBy(msg => msg.Key));
		}
	}
}