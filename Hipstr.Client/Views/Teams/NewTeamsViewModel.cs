using Hipstr.Client.Commands;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.EditTeamDialog;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
    [UsedImplicitly]
	public class NewTeamsViewModel : ViewModelBase
	{
		public ObservableCollection<Team> Teams { get; }
		public ICommand EditTeamCommand { get; }
		public ICommand DeleteTeamCommand { get; }

		private readonly ITeamService _teamService;

		public NewTeamsViewModel(ITeamService teamService)
		{
			_teamService = teamService;

			Teams = new ObservableCollection<Team>();

			EditTeamCommand = new RelayCommandAsync<Team>(EditTeamAsync, team => team != null);
			DeleteTeamCommand = new RelayCommandAsync<Team>(DeleteTeamAsync, team => team != null);
		}

		public override void Initialize()
		{
			base.Initialize();

			Eventing.TeamAdded += OnTeamAdded;
		}

        public override void Dispose()
        {
            base.Dispose();

            Eventing.TeamAdded -= OnTeamAdded;
        }

        private void OnTeamAdded(object sender, TeamAddedEventArgs teamAddedEventArgs)
		{
			Teams.Add(teamAddedEventArgs.Team);
		}

		public override async Task InitializeAsync()
		{
			Teams.AddRange(await _teamService.GetTeamsAsync());
		}

		private async Task EditTeamAsync(Team selectedTeam)
		{
			var dialog = new EditTeamDialogView();
			DialogResult<Team> teamResult = await dialog.ShowAsync(selectedTeam);
			if (!teamResult.Cancelled)
			{
				string teamName = teamResult.Result.Name;
				string apiKey = selectedTeam.ApiKey;

				var team = new Team(teamName, apiKey);
				await _teamService.EditTeamAsync(team);
				Teams.Insert(Teams.IndexOf(selectedTeam), team);
				Teams.Remove(selectedTeam);
			}
		}

		private async Task DeleteTeamAsync(Team team)
		{
			await _teamService.RemoveTeamAsync(team);
            Eventing.FireTeamDeletedEvent(this, team);
			Teams.Remove(team);
		}
	}
}