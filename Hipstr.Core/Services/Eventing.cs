using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
	public static class Eventing
	{
		public static event TeamAddedEventHandler TeamAdded;

		public delegate void TeamAddedEventHandler(object sender, TeamAddedEventArgs e);

		public static void FireTeamAddedEvent(object sender, Team team)
		{
			TeamAdded?.Invoke(sender, new TeamAddedEventArgs(team));
		}
	}

	public class TeamAddedEventArgs
	{
		public Team Team { get; }

		public TeamAddedEventArgs(Team team)
		{
			Team = team;
		}
	}
}