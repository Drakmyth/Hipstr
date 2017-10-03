using Hipstr.Client.Commands;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.EditTeamDialog;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using JetBrains.Annotations;
using System;
using System.Collections.ObjectModel;
using System.Linq;
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

		private Team _tappedTeam;

		public Team TappedTeam
		{
			get => _tappedTeam;
			set
			{
				_tappedTeam = value;
				OnPropertyChanged();
			}
		}

		private readonly ITeamService _teamService;

		public NewTeamsViewModel(ITeamService teamService)
		{
			_teamService = teamService;

			Teams = new ObservableCollection<Team>();

			EditTeamCommand = new RelayCommandAsync<Team>(EditTeamAsync, team => team != null, this, nameof(TappedTeam));
			DeleteTeamCommand = new RelayCommandAsync<Team>(DeleteTeamAsync, team => team != null, this, nameof(TappedTeam));
		}

		public override void Initialize()
		{
			base.Initialize();

			Eventing.TeamAdded += OnTeamAdded;
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
			DialogResult<Team> team = await dialog.ShowAsync(selectedTeam);
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = selectedTeam.ApiKey;

				var team2 = new Team(teamName, apiKey);
				await _teamService.EditTeamAsync(team2);
				Teams.Insert(Teams.IndexOf(selectedTeam), team2);
				Teams.Remove(selectedTeam);
			}
		}

		private async Task DeleteTeamAsync(Team team)
		{
			await _teamService.RemoveTeamAsync(team);
			Teams.Remove(team);
		}

		private async Task RefreshTeamListAsync()
		{
			Teams.Clear();
			Teams.AddRange(await _teamService.GetTeamsAsync());
		}
	}
}