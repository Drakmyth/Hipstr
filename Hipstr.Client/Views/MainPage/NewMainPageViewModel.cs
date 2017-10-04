using Hipstr.Client.Commands;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.AddTeamDialog;
using Hipstr.Client.Views.Dialogs.JoinChatDialog;
using Hipstr.Client.Views.Subscriptions;
using Hipstr.Client.Views.Teams;
using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	[UsedImplicitly]
	public class NewMainPageViewModel : ViewModelBase
	{
		public ICommand AddTeamCommand { get; }
		public ICommand JoinRoomCommand { get; }
		public ICommand NavigateToTeamsCommand { get; }
		public ICommand NavigateToSubscriptionsCommand { get; }

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;
		private readonly ISubscriptionService _subscriptionService;

		public NewMainPageViewModel(ITeamService teamService, IHipChatService hipChatService, ISubscriptionService subscriptionService)
		{
			_teamService = teamService;
			_hipChatService = hipChatService;
			_subscriptionService = subscriptionService;

			AddTeamCommand = new RelayCommandAsync(AddTeamAsync);
			JoinRoomCommand = new RelayCommandAsync(JoinRoomAsync);
			NavigateToTeamsCommand = new NavigateToViewCommand<NewTeamsView>();
			NavigateToSubscriptionsCommand = new NavigateToViewCommand<SubscriptionsView>();
		}

        private async Task AddTeamAsync()
		{
			var dialog = new AddTeamDialogView();
			DialogResult<Team> team = await dialog.ShowAsync();
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = team.Result.ApiKey;

				var team2 = new Team(teamName, apiKey);
				await _teamService.AddTeamAsync(team2);
				Eventing.FireTeamAddedEvent(this, team2);
			}
		}

		private async Task JoinRoomAsync()
		{
			var dialog = new JoinChatDialogView("Join Room");

			IReadOnlyList<Team> teams = await _teamService.GetTeamsAsync();

			var roomTasks = new List<Task<IReadOnlyList<Room>>>();

			foreach (Team team in teams)
			{
				Task<IReadOnlyList<Room>> task = _hipChatService.GetRoomsForTeamAsync(team);
				roomTasks.Add(task);
			}

			await Task.WhenAll(roomTasks);

			var rooms = new List<IMessageSource>();
			foreach (Task<IReadOnlyList<Room>> roomTask in roomTasks)
			{
				rooms.AddRange(roomTask.Result.Select(room => new RoomMessageSource(_hipChatService, room)));
			}

			DialogResult<IMessageSource> roomResult = await dialog.ShowAsync(rooms);
			if (!roomResult.Cancelled)
			{
				var roomMessageSource = (RoomMessageSource)roomResult.Result;
				await _subscriptionService.AddSubscriptionAsync(roomMessageSource);
				Eventing.FireRoomJoinedEvent(this, roomMessageSource.Room);
			}
		}
	}
}