using Hipstr.Client.Commands.ViewCommands;
using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel
	{
		public List<Team> Teams { get; set; }

		public ICommand AddTeamCommand { get; set; }

		public TeamsViewModel()
		{
			Teams = new List<Team> {
				new Team("Slalom PDP"),
				new Team("ExtraSpace Storage")
			};

			AddTeamCommand = new NavigateToAddTeamViewCommand();
		}
	}
}