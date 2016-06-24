﻿using Hipstr.Client.Commands.ViewCommands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel
	{
		public ObservableCollection<Team> Teams { get; set; }
		public ICommand AddTeamCommand { get; set; }

		private readonly ITeamService _teamService;

		public TeamsViewModel() : this(IoCContainer.Resolve<ITeamService>()) { }
		public TeamsViewModel(ITeamService teamService)
		{
			_teamService = teamService;

			// TODO: Pull this into loading persistance
			// TODO: Make it not required to view team list before rooms will load
			_teamService.AddTeam(new Team("---Default Team Name---", "---API KEY goes here---")); // API_KEY is good for 1 year from generation date.

			RefreshTeamList();

			AddTeamCommand = new NavigateToAddTeamViewCommand();
		}

		private void RefreshTeamList()
		{
			Teams = new ObservableCollection<Team>(_teamService.GetTeams());
		}
	}
}