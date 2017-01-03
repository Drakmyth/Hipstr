using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Hipstr.Core.Models.Collections
{
	public class ObservableGroupedCollection<T> : ObservableCollection<T>
	{
		public string Header { get; }

		public ObservableGroupedCollection(string header, IEnumerable<T> items = null) : base(items ?? Enumerable.Empty<T>())
		{
			Header = header;
		}
	}
}