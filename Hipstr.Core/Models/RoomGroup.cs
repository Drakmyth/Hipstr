using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Core.Models
{
	public class RoomGroup
	{
		public string Header { get; }
		public ObservableCollection<Room> Rooms { get; }

		public RoomGroup(string header, IEnumerable<Room> rooms = null)
		{
			Header = header;
			Rooms = rooms == null ? new ObservableCollection<Room>() : new ObservableCollection<Room>(rooms);
		}
	}
}