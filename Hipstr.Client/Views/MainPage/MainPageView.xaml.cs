using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class MainPageView : Page
	{
		public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

		public MainPageView(UIElement frame)
		{
			InitializeComponent();
			((Grid)MenuSplitView.Content)?.Children.Add(frame);
			DataContext = IoCContainer.Resolve<MainPageViewModel>();
		}

		private void MenuButton_Click(object sender, RoutedEventArgs e)
		{
			ViewModel.MenuIsOpen = false;
		}
	}
}