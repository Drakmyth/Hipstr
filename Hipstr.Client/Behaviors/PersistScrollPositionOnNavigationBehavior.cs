using Microsoft.Xaml.Interactivity;
using System;
using System.Collections;
using System.Reflection;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Behaviors
{
	public class PersistScrollPositionOnNavigationBehavior : Trigger<ListView>
	{
		private static string _persistedPosition = "";
		private static IPersistScrollAdapter _adapter;

		public static readonly DependencyProperty SourceObjectProperty = DependencyProperty.Register(
			nameof(SourceObject),
			typeof(object),
			typeof(PersistScrollPositionOnNavigationBehavior),
			new PropertyMetadata(null));

		public static readonly DependencyProperty EventNameProperty = DependencyProperty.Register(
			nameof(EventName),
			typeof(string),
			typeof(PersistScrollPositionOnNavigationBehavior),
			new PropertyMetadata(""));

		public static readonly DependencyProperty PersistScrollAdapterProperty = DependencyProperty.Register(
			nameof(Adapter),
			typeof(IPersistScrollAdapter),
			typeof(PersistScrollPositionOnNavigationBehavior),
			new PropertyMetadata(null, OnPersistScrollAdapterChanged));

		private static void OnPersistScrollAdapterChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			_adapter = (IPersistScrollAdapter)e.NewValue;
		}

		public object SourceObject
		{
			get { return GetValue(SourceObjectProperty); }
			set { SetValue(SourceObjectProperty, value); }
		}

		public string EventName
		{
			get { return (string)GetValue(EventNameProperty); }
			set { SetValue(EventNameProperty, value); }
		}

		public IPersistScrollAdapter Adapter
		{
			get { return (IPersistScrollAdapter)GetValue(PersistScrollAdapterProperty); }
			set { SetValue(PersistScrollAdapterProperty, value); }
		}

		protected override void OnAttached()
		{
			base.OnAttached();
			AssociatedObject.Loaded += AssociatedObject_OnLoaded;
		}

		private void AssociatedObject_OnLoaded(object sender, RoutedEventArgs routedEventArgs)
		{
			AssociatedObject.Loaded -= AssociatedObject_OnLoaded;

			// TODO: If no SourceObject/EventName, call OnCollectionLoadedEvent here with direct contents of AssociatedObject list

			EventInfo ev = SourceObject.GetType().GetEvent(EventName);
			Action<object, IEnumerable> handler = OnCollectionLoadedEvent;
			Delegate d = handler.GetMethodInfo().CreateDelegate(ev.EventHandlerType, this);
			ev.AddEventHandler(SourceObject, d);
		}

		protected override void OnDetaching()
		{
			base.OnDetaching();
			_persistedPosition = ListViewPersistenceHelper.GetRelativeScrollPosition(AssociatedObject, _adapter.GetKey);
		}

		private async void OnCollectionLoadedEvent(object sender, IEnumerable collection)
		{
			if (string.IsNullOrEmpty(_persistedPosition)) return;

			ListViewKeyToItemHandler getItem = delegate(string key) { return Task.Run(() => _adapter.GetItem(key, collection)).AsAsyncOperation(); };
			await ListViewPersistenceHelper.SetRelativeScrollPositionAsync(AssociatedObject, _persistedPosition, getItem);
		}
	}
}