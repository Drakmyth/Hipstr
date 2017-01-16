using Hipstr.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Behaviors
{
	public class RoomPersistScrollAdapter : IPersistScrollAdapter
	{
		public object GetItem(string key, IEnumerable collection)
		{
			List<ObservableGroupedCollection<Room>> groups = ((IEnumerable<ObservableGroupedCollection<Room>>)collection).ToList();
			var groupMappings = groups.Select(group => new {Key = GetKey(group), Group = group});

			var groupKeyMapping = groupMappings.Where(mapping => mapping.Key == key).SingleOrDefault();
			if (groupKeyMapping != null) return groupKeyMapping.Group;

			foreach (ObservableGroupedCollection<Room> group in groups)
			{
				var roomMappings = group.Select(room => new {Key = GetKey(room), Room = room});

				var roomKeyMapping = roomMappings.Where(mapping => mapping.Key == key).SingleOrDefault();
				if (roomKeyMapping != null) return roomKeyMapping.Room;
			}

			return null;
		}

		public string GetKey(object item)
		{
			var room = item as Room;
			if (room != null)
			{
				return room.Id.ToString();
			}

			var group = item as ObservableGroupedCollection<Room>;
			return group?.Header;
		}
	}
}