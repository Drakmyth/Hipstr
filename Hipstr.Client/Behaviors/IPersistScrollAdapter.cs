using System.Collections;
using System.Collections.Generic;

namespace Hipstr.Client.Behaviors
{
	public interface IPersistScrollAdapter
	{
		object GetItem(string key, IEnumerable collection);
		string GetKey(object item);
	}
}