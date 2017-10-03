using Hipstr.Core.Models;

namespace Hipstr.Core.Services
{
	public static class Eventing
	{
		public static event TeamAddedEventHandler TeamAdded;
		public static event RoomJoinedEventHandler RoomJoined;

		public delegate void TeamAddedEventHandler(object sender, TeamAddedEventArgs e);

		public delegate void RoomJoinedEventHandler(object sender, RoomJoinedEventArgs e);

		public static void FireTeamAddedEvent(object sender, Team team)
		{
			TeamAdded?.Invoke(sender, new TeamAddedEventArgs(team));
		}

		public static void FireRoomJoinedEvent(object sender, Room room)
		{
			RoomJoined?.Invoke(sender, new RoomJoinedEventArgs(room));
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

	public class RoomJoinedEventArgs
	{
		public Room Room { get; }

		public RoomJoinedEventArgs(Room room)
		{
			Room = room;
		}
	}
}