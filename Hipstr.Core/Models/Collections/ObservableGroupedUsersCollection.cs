using System.Collections.Generic;

namespace Hipstr.Core.Models.Collections
{
	/// <summary>
	/// Represents a grouped collection of Users. This class exists to enable XAML binding since UWP doesn't support XAML 2009 and thus doesn't support type arguments.
	/// As of right now, there is no custom logic specific to users to warrant a specialized collection.
	/// </summary>
	public class ObservableGroupedUsersCollection : ObservableGroupedCollection<User>
	{
		public ObservableGroupedUsersCollection(string header, IEnumerable<User> items = null) : base(header, items)
		{
		}
	}
}