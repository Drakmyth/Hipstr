using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

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

		public static void AddRange<T>(this ObservableCollection<T> list, IEnumerable<T> collection)
		{
			IEnumerable<T> enumerable = collection as IList<T> ?? collection.ToList();

			const string EVENT_NAME = nameof(list.CollectionChanged);
			Type ocType = typeof(ObservableCollection<T>);
			FieldInfo eventField = ocType.GetField(EVENT_NAME, BindingFlags.Instance | BindingFlags.NonPublic);
			var eventHandler = (NotifyCollectionChangedEventHandler)eventField.GetValue(list);
			Delegate[] eventDelegates = eventHandler.GetInvocationList().Where(eh => eh.Target != null).ToArray();
			EventInfo eventInfo = ocType.GetEvent(EVENT_NAME);

			T last = enumerable.Last();
			foreach (Delegate d in eventDelegates)
			{
				eventInfo.RemoveEventHandler(list, d);
			}
			AddRange(list as IList<T>, enumerable.Take(enumerable.Count() - 1));
			foreach (Delegate d in eventDelegates)
			{
				eventInfo.AddEventHandler(list, d);
			}
			list.Add(last);
		}
	}
}