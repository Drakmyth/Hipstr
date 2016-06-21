using Hipstr.Core.Services;
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
	}
}
