using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Views.Teams
{
    public sealed partial class NewTeamsView : Page
    {
        public NewTeamsViewModel ViewModel => (NewTeamsViewModel)DataContext;

        public NewTeamsView()
        {
            InitializeComponent();
            DataContext = IoCContainer.Resolve<NewTeamsViewModel>();
        }

        public Visibility ShowEmptyCollectionMessage(int collectionCount)
        {
            return collectionCount > 0 ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}