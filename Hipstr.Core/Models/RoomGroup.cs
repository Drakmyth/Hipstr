using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Core.Models
{
	public class RoomGroup
	{
		public string Header { get; }
		public ObservableCollection<Room> Rooms { get; }

		public RoomGroup(string header, IEnumerable<Room> rooms)
		{
			Header = header;
			Rooms = new ObservableCollection<Room>(rooms);
		}
	}
}