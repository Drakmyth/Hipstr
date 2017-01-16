using Hipstr.Core.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Client.Behaviors
{
	public class UserPersistScrollAdapter : IPersistScrollAdapter
	{
		public object GetItem(string key, IEnumerable collection)
		{
			List<ObservableGroupedCollection<User>> groups = ((IEnumerable<ObservableGroupedCollection<User>>)collection).ToList();
			var groupMappings = groups.Select(group => new {Key = GetKey(group), Group = group}).ToList();

			var groupKeyMapping = groupMappings.Where(mapping => mapping.Key == key).SingleOrDefault();
			if (groupKeyMapping != null) return groupKeyMapping.Group;

			foreach (ObservableGroupedCollection<User> group in groups)
			{
				var userMappings = group.Select(user => new {Key = GetKey(user), User = user}).ToList();

				var userKeyMapping = userMappings.Where(mapping => mapping.Key == key).SingleOrDefault();
				if (userKeyMapping != null) return userKeyMapping.User;
			}

			return null;
		}

		public string GetKey(object item)
		{
			var user = item as User;
			if (user != null)
			{
				return user.Id.ToString();
			}

			var group = item as ObservableGroupedCollection<User>;
			return group?.Header;
		}
	}
}