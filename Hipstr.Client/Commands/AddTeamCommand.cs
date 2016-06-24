using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class AddTeamCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly ITeamService _teamService;

		public AddTeamCommand() : this(IoCContainer.Resolve<ITeamService>()) { }
		public AddTeamCommand(ITeamService teamService)
		{
			_teamService = teamService;
		}

		public bool CanExecute(object parameter)
		{
			Team teamToAdd = (Team)parameter;
			return !_teamService.TeamExists(teamToAdd.ApiKey);
		}

		public void Execute(object parameter)
		{
			Team teamToAdd = (Team)parameter;
			_teamService.AddTeam(teamToAdd);
		}
	}
}
