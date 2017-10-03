using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Hipstr.Core.Messaging
{
	public class MessageSourceGroup : IGrouping<string, IMessageSource>
	{
		private readonly List<IMessageSource> _messageSources;
		public string Key { get; }

		public MessageSourceGroup(string key, IEnumerable<IMessageSource> messageSources)
		{
			Key = key;
			_messageSources = new List<IMessageSource>(messageSources);
		}

		public IEnumerator<IMessageSource> GetEnumerator() => _messageSources.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _messageSources.GetEnumerator();
	}
}