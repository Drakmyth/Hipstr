using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Hipstr.Client.Services;
using JetBrains.Annotations;

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

		public TeamsViewModel(ITeamService teamService, IMainPageService mainPageService)
		{
			_teamService = teamService;

			Teams = new ObservableCollection<Team>();
			mainPageService.Title = "Teams";

			AddTeamCommand = new RelayCommandAsync(AddTeamAsync);
			EditTeamCommand = new RelayCommandAsync<Team>(EditTeamAsync, team => team != null);
			DeleteTeamCommand = new RelayCommandAsync<Team>(DeleteTeamAsync, team => team != null);
		}

		private async Task AddTeamAsync()
		{
			var dialog = new AddTeamDialog();
			ModalResult<Team> team = await dialog.ShowAsync();
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = team.Result.ApiKey;

				// TODO: Duplicate API Key should show error message

				await _teamService.AddTeamAsync(new Team(teamName, apiKey));
				await RefreshTeamListAsync();
			}
		}

		private async Task EditTeamAsync(Team selectedTeam)
		{
			var dialog = new EditTeamDialog();
			ModalResult<Team> team = await dialog.ShowAsync(selectedTeam);
			if (!team.Cancelled)
			{
				string teamName = team.Result.Name;
				string apiKey = team.Result.ApiKey;

				await _teamService.EditTeamAsync(selectedTeam.ApiKey, new Team(teamName, apiKey));
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