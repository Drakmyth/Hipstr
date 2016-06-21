using Hipstr.Client.Views.Teams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Commands.ViewCommands
{
	public class NavigateToAddTeamViewCommand : NavigateToViewCommand<AddTeamView>
	{
		public override bool CanExecute(object parameter)
		{
			return ((Frame)Window.Current.Content).CurrentSourcePageType != typeof(AddTeamView);
		}

		public override void Execute(object parameter)
		{
			((Frame)Window.Current.Content).Navigate(typeof(AddTeamView));
		}
	}
}