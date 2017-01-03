using System.Collections.Generic;

namespace Hipstr.Core.Models.Collections
{
	/// <summary>
	/// Represents a grouped collection of Rooms. This class exists to enable XAML binding since UWP doesn't support XAML 2009 and thus doesn't support type arguments.
	/// As of right now, there is no custom logic specific to rooms to warrant a specialized collection.
	/// </summary>
	public class ObservableGroupedRoomsCollection : ObservableGroupedCollection<Room>
	{
		public ObservableGroupedRoomsCollection(string header, IEnumerable<Room> items = null) : base(header, items)
		{
		}
	}
}