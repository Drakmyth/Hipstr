using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using Hipstr.Core.Utility.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel : ViewModelBase, ITitled
	{
		public string Title => "Teams";
		public ObservableCollection<Team> Teams { get; set; }
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

		public TeamsViewModel() : this(IoCContainer.Resolve<ITeamService>())
		{
		}

		public TeamsViewModel(ITeamService teamService)
		{
			_teamService = teamService;
			Teams = new ObservableCollection<Team>();

			AddTeamCommand = new RelayCommandAsync(OnAddTeamCommand);
			EditTeamCommand = new RelayCommandAsync<Team>(OnEditTeamCommandAsync, team => team != null);
			DeleteTeamCommand = new RelayCommandAsync<Team>(OnDeleteTeamCommandAsync, team => team != null);
		}

		private async Task OnAddTeamCommand()
		{
			var dialog = new AddTeamDialog();
			ModalResult<Team> team = await dialog.ShowAsync();
			if (!team.Cancelled)
			{
				string teamName = dialog.TeamName;
				string apiKey = dialog.ApiKey;

				// TODO: Duplicate API Key should show error message

				await _teamService.AddTeamAsync(new Team(teamName, apiKey));
				await RefreshTeamListAsync();
			}
		}

		private async Task OnEditTeamCommandAsync(Team selectedTeam)
		{
			var dialog = new EditTeamDialog(selectedTeam);
			ModalResult<Team> team = await dialog.ShowAsync();
			if (!team.Cancelled)
			{
				string teamName = dialog.TeamName;
				string apiKey = dialog.ApiKey;

				await _teamService.EditTeamAsync(selectedTeam.ApiKey, new Team(teamName, apiKey));
				await RefreshTeamListAsync();
			}
		}

		private async Task OnDeleteTeamCommandAsync(Team team)
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