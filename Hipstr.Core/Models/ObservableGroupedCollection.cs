using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hipstr.Core.Models
{
	// Right now, UWP doesn't support XAML 2009 and thus doesn't support using type parameters.
	// In order to bind to these correctly then, we must create subclasses of this typed to
	// specific type parameters and then bind to those.
	public abstract class ObservableGroupedCollection<T> : ObservableCollection<T>
	{
		public string Header { get; }

		protected ObservableGroupedCollection(string header, IEnumerable<T> items = null) : base(items ?? Enumerable.Empty<T>())
		{
			Header = header;
		}
	}

	// TODO: Delete this class and use ObservableGroupedCollection<Room> directly once UWP supports XAML 2009
	public class ObservableGroupedRoomsCollection : ObservableGroupedCollection<Room>
	{
		public ObservableGroupedRoomsCollection(string header, IEnumerable<Room> items = null) : base(header, items)
		{
		}
	}

	// TODO: Delete this class and use ObservableGroupedCollection<User> directly once UWP supports XAML 2009
	public class ObservableGroupedUsersCollection : ObservableGroupedCollection<User>
	{
		public ObservableGroupedUsersCollection(string header, IEnumerable<User> items = null) : base(header, items)
		{
		}
	}
}