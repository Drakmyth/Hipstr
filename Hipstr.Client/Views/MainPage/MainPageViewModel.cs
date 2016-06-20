using Hipstr.Client.Commands;
using System.Windows.Input;

namespace Hipstr.Client.Views.MainPage
{
	public class MainPageViewModel
	{
		public ICommand TeamsChangeViewCommand { get; set; }
		public ICommand RoomsChangeViewCommand { get; set; }
		public ICommand PeopleChangeViewCommand { get; set; }

		public MainPageViewModel()
		{
			TeamsChangeViewCommand = new SwitchToTeamsViewCommand();
			RoomsChangeViewCommand = new SwitchToRoomsViewCommand();
			PeopleChangeViewCommand = new SwitchToPeopleViewCommand();
		}
	}
}