using System.Collections.Generic;

namespace Hipstr.Core.Models
{
	public class Subscription
	{
		private readonly Dictionary<string, List<Room>> _rooms;
		private readonly Dictionary<string, List<User>> _users;

		public Dictionary<string, IEnumerable<Room>> Rooms { get; set; }
		public Dictionary<string, IEnumerable<User>> Users { get; set; }

		public Subscription()
		{
			Rooms = new Dictionary<string, IEnumerable<Room>>();
			Users = new Dictionary<string, IEnumerable<User>>();
			_rooms = new Dictionary<string, List<Room>>();
			_users = new Dictionary<string, List<User>>();
		}

		public void AddRoom(Room room)
		{
			if (!_rooms.ContainsKey(room.Team.ApiKey))
			{
				var roomList = new List<Room>();

				_rooms.Add(room.Team.ApiKey, roomList);
				Rooms.Add(room.Team.ApiKey, roomList);
			}

			_rooms[room.Team.ApiKey].Add(room);
		}

		public void AddUser(User user)
		{
			if (!_users.ContainsKey(user.Team.ApiKey))
			{
				var userList = new List<User>();

				_users.Add(user.Team.ApiKey, userList);
				Users.Add(user.Team.ApiKey, userList);
			}

			_users[user.Team.ApiKey].Add(user);
		}
	}
}