using Hipstr.Client.Commands;
using Hipstr.Client.Services;
using Hipstr.Client.Views.Dialogs;
using Hipstr.Client.Views.Dialogs.AddTeamDialog;
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
	public class TeamsViewModel : ViewModelBase
	{
		public ObservableCollection<Team> Teams { get; }
		public ICommand AddTeamCommand { get; }
		public ICommand EditTeamCommand { get; }
		public ICommand DeleteTeamCommand { get; }

		private Team _tappedTeam;

		public Team TappedTeam
		{
			get { return _tappedTeam; }
			set
			{
				_tappedTeam = value;
				OnPropertyChanged();
			}
		}

		private readonly ITeamService _teamService;

		public TeamsViewModel(ITeamService teamService) : base("Teams")
		{
			_teamService = teamService;

			Teams = new ObservableCollection<Team>();

			AddTeamCommand = new RelayCommandAsync(AddTeamAsync);
			EditTeamCommand = new RelayCommandAsync<Team>(EditTeamAsync, team => team != null, this, nameof(TappedTeam));
			DeleteTeamCommand = new RelayCommandAsync<Team>(DeleteTeamAsync, team => team != null, this, nameof(TappedTeam));
		}

		private async Task AddTeamAsync()
		{
			var dialog = new AddTeamDialogView();
			DialogResult<Team> team = await dialog.ShowAsync();
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = team.Result.ApiKey;

				await _teamService.AddTeamAsync(new Team(teamName, apiKey));
				await RefreshTeamListAsync();
			}
		}

		private async Task EditTeamAsync(Team selectedTeam)
		{
			var dialog = new EditTeamDialogView();
			DialogResult<Team> team = await dialog.ShowAsync(selectedTeam);
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = selectedTeam.ApiKey;

				await _teamService.EditTeamAsync(new Team(teamName, apiKey));
				await RefreshTeamListAsync();
			}
		}

		private async Task DeleteTeamAsync(Team team)
		{
			await _teamService.RemoveTeamAsync(team);
			await RefreshTeamListAsync();
		}

		public async Task RefreshTeamListAsync()
		{
			Teams.Clear();
			Teams.AddRange(await _teamService.GetTeamsAsync());
		}
	}
}