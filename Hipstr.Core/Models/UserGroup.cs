using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Hipstr.Core.Models
{
	public class UserGroup
	{
		public string Header { get; }
		public ObservableCollection<User> Users { get; }

		public UserGroup(string header, IEnumerable<User> users)
		{
			Header = header;
			Users = new ObservableCollection<User>(users);
		}
	}
}