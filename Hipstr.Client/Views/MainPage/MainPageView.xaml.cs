using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Shapes;

namespace Hipstr.Client.Views.MainPage
{
	public sealed partial class MainPageView : Page
	{
		public MainPageViewModel ViewModel => (MainPageViewModel)DataContext;

		public MainPageView()
		{
			InitializeComponent();
			DataContext = IoCContainer.Resolve<MainPageViewModel>();
		}
	}
}