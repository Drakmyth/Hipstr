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
		public ICommand JoinUserCommand { get; }
		public ICommand NavigateToTeamsCommand { get; }
		public ICommand NavigateToSubscriptionsCommand { get; }

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;
		private readonly ISubscriptionService _subscriptionService;

		// TODO: this list exists to evaluate 'AreTeamsJoined' on Add/Remove Team.
		// See if we can find a better way of doing this that doesn't involve storing data we don't need.
		private readonly List<Team> _teams;

		public NewMainPageViewModel(ITeamService teamService, IHipChatService hipChatService, ISubscriptionService subscriptionService)
		{
			_teamService = teamService;
			_hipChatService = hipChatService;
			_subscriptionService = subscriptionService;
			_teams = new List<Team>();

			AddTeamCommand = new RelayCommandAsync(AddTeamAsync);
			JoinUserCommand = new RelayCommandAsync(JoinUserAsync, AreTeamsJoined);
			JoinRoomCommand = new RelayCommandAsync(JoinRoomAsync, AreTeamsJoined);
			NavigateToTeamsCommand = new NavigateToViewCommand<NewTeamsView>();
			NavigateToSubscriptionsCommand = new NavigateToViewCommand<SubscriptionsView>();
		}

		public override void Initialize()
		{
			base.Initialize();

			Eventing.TeamDeleted += OnTeamDeleted;
		}

		public override async Task InitializeAsync()
		{
			_teams.AddRange(await _teamService.GetTeamsAsync());
		}

		public override void Dispose()
		{
			Eventing.TeamDeleted -= OnTeamDeleted;

			base.Dispose();
		}

		private bool AreTeamsJoined()
		{
			return _teams.Count > 0;
		}

		private void OnTeamDeleted(object sender, TeamDeletedEventArgs teamDeletedEventArgs)
		{
			_teams.Remove(teamDeletedEventArgs.Team);

			((RelayCommandAsync)JoinRoomCommand).EvaluateCanExecute(this);
			((RelayCommandAsync)JoinUserCommand).EvaluateCanExecute(this);
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
				_teams.Add(team2);

				((RelayCommandAsync)JoinRoomCommand).EvaluateCanExecute(this);
				((RelayCommandAsync)JoinUserCommand).EvaluateCanExecute(this);
			}
		}

		private async Task JoinRoomAsync()
		{
			IReadOnlyList<IMessageSource> subscriptions = await _subscriptionService.GetSubscriptionsAsync(_hipChatService);
			var dialog = new JoinChatDialogView("Join Room", subscriptions);

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

		private async Task JoinUserAsync()
		{
			IReadOnlyList<IMessageSource> subscriptions = await _subscriptionService.GetSubscriptionsAsync(_hipChatService);
			var dialog = new JoinChatDialogView("Chat With User", subscriptions);

			IReadOnlyList<Team> teams = await _teamService.GetTeamsAsync();

			var userTasks = new List<Task<IReadOnlyList<User>>>();

			foreach (Team team in teams)
			{
				Task<IReadOnlyList<User>> task = _hipChatService.GetUsersForTeamAsync(team);
				userTasks.Add(task);
			}

			await Task.WhenAll(userTasks);

			var users = new List<IMessageSource>();
			foreach (Task<IReadOnlyList<User>> userTask in userTasks)
			{
				users.AddRange(userTask.Result.Select(user => new UserMessageSource(_hipChatService, user)));
			}

			DialogResult<IMessageSource> userResult = await dialog.ShowAsync(users);
			if (!userResult.Cancelled)
			{
				var userMessageSource = (UserMessageSource)userResult.Result;
				await _subscriptionService.AddSubscriptionAsync(userMessageSource);
				Eventing.FireUserJoinedEvent(this, userMessageSource.User);
			}
		}
	}
}