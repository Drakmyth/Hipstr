﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Hipstr.Client.Views.Subscriptions
{
	public sealed partial class SubscriptionsView : Page
	{
		public SubscriptionsViewModel ViewModel => (SubscriptionsViewModel)DataContext;

		public SubscriptionsView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<SubscriptionsViewModel>();
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e)
		{
			base.OnNavigatedFrom(e);

			GroupZoom.IsZoomedInViewActive = true;
		}

		public Visibility ShowEmptyCollectionMessage(int collectionCount)
		{
			return collectionCount > 0 ? Visibility.Collapsed : Visibility.Visible;
		}
	}
}