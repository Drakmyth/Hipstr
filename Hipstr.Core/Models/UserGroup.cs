using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Core.Models
{
	public class UserGroup
	{
		public string Header { get; }
		public ObservableCollection<User> Users { get; }

		public UserGroup(string header, IEnumerable<User> users = null)
		{
			Header = header;
			Users = users == null ? new ObservableCollection<User>() : new ObservableCollection<User>(users);
		}
	}
}