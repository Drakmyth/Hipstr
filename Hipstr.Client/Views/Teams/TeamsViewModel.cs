using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel : ViewModelBase, ITitled
	{
		public string Title => "Teams";
		public ObservableCollection<Team> Teams { get; set; }
		public ICommand AddTeamCommand { get; set; }

		private readonly ITeamService _teamService;

		public TeamsViewModel() : this(IoCContainer.Resolve<ITeamService>())
		{
		}

		public TeamsViewModel(ITeamService teamService)
		{
			_teamService = teamService;
			Teams = new ObservableCollection<Team>();

			RefreshTeamList().Wait();

			AddTeamCommand = new RelayCommand(async () =>
			{
				var dialog = new AddTeamDialog();
				ModalResult<Team> team = await dialog.ShowAsync();
				if (!team.Cancelled)
				{
					string teamName = dialog.TeamName;
					string apiKey = dialog.ApiKey;

					_teamService.AddTeamAsync(new Team(teamName, apiKey));
					await RefreshTeamList();
				}
			});
		}

		private async Task RefreshTeamList()
		{
			Teams.Clear();
			IEnumerable<Team> teams = await _teamService.GetTeamsAsync();
			foreach (Team team in teams)
			{
				Teams.Add(team);
			}
		}
	}
}