using Hipstr.Client.Commands.ViewCommands;
using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel
	{
		public ObservableCollection<Team> Teams { get; set; }

		public ICommand AddTeamCommand { get; set; }

		public TeamsViewModel()
		{
			Teams = new ObservableCollection<Team>(new List<Team> {
				new Team("Slalom PDP", "API key 1"),
				new Team("ExtraSpace Storage", "Api key 2")
			});

			AddTeamCommand = new NavigateToAddTeamViewCommand();
		}
	}
}