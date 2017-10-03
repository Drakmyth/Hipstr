using Hipstr.Client.Commands;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.AddTeamDialog;
using Hipstr.Client.Views.Subscriptions;
using Hipstr.Client.Views.Teams;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	[UsedImplicitly]
	public class NewMainPageViewModel : ViewModelBase
	{
		public ICommand AddTeamCommand { get; }
		public ICommand NavigateToTeamsCommand { get; }
		public ICommand NavigateToSubscriptionsCommand { get; }

		private readonly ITeamService _teamService;

		public NewMainPageViewModel(ITeamService teamService)
		{
			_teamService = teamService;

			AddTeamCommand = new RelayCommandAsync(AddTeamAsync);
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
//				await RefreshTeamListAsync();
			}
		}
	}
}