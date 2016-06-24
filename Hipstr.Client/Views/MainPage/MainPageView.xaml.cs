using Hipstr.Core.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class MainPageView : Page
	{
		public MainPageView() : this(IoCContainer.Resolve<INavigationService>()) { }
		private MainPageView(INavigationService navigationService)
		{
			InitializeComponent();
			navigationService.SetFrame(ContentFrame);
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
