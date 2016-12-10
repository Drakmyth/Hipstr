using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class MainPageView : Page
	{
		public MainPageViewModel ViewModel => DataContext as MainPageViewModel;

		// TODO: Consider creating a new ILifetimeScope in OnNavigatedTo and disposing it in OnNavigatedFrom for each Page

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

		private void MenuButton_Click(object sender, RoutedEventArgs e)
		{
			MenuSplitView.IsPaneOpen = false;
		}
	}
}
