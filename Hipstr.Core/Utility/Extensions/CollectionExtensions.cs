using System.Collections.Generic;

namespace Hipstr.Core.Utility.Extensions
{
	public static class CollectionExtensions
	{
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> collection)
		{
			foreach (T item in collection)
			{
				list.Add(item);
			}
		}
	}
}