﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class MainPageView : Page
	{
		public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

		public MainPageView(UIElement frame)
		{
			InitializeComponent();
			MenuSplitView.Content = frame;
			DataContext = new MainPageViewModel();
		}

		private void HamburgerButton_Click(object sender, RoutedEventArgs e)
		{
			MenuSplitView.IsPaneOpen = !MenuSplitView.IsPaneOpen;
		}

		private void RadioButton_Click(object sender, RoutedEventArgs e)
		{
			MenuSplitView.IsPaneOpen = false;
		}
	}
}
