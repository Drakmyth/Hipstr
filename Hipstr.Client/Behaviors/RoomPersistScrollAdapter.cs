using Hipstr.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Behaviors
{
	public class RoomPersistScrollAdapter : IPersistScrollAdapter<Room>
	{
		public Room GetItem(string key, IEnumerable<Room> collection)
		{
			return collection.Where(room => GetKey(room) == key).SingleOrDefault();
		}

		private static ObservableGroupedCollection<Room> GetItem(string key, IEnumerable<ObservableGroupedCollection<Room>> collection)
		{
			return collection.Where(group => GetKey(group) == key).SingleOrDefault();
		}

		public object GetItem(string key, IEnumerable collection)
		{
			if (collection is IEnumerable<Room>)
			{
				return GetItem(key, (IEnumerable<Room>)collection);
			}

			if (collection is IEnumerable<ObservableGroupedCollection<Room>>)
			{
				return GetItem(key, (IEnumerable<ObservableGroupedCollection<Room>>)collection);
			}

			return null;
		}

		public string GetKey(Room room)
		{
			return room.Id.ToString();
		}

		private static string GetKey(ObservableGroupedCollection<Room> group)
		{
			return group.Header;
		}

		public string GetKey(object item)
		{
			if (item is Room)
			{
				return GetKey((Room)item);
			}

			if (item is ObservableGroupedCollection<Room>)
			{
				return GetKey((ObservableGroupedCollection<Room>)item);
			}

			return null;
		}
	}
}