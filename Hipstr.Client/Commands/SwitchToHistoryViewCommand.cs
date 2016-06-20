using Hipstr.Client.Views.Teams;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Hipstr.Client.Views.Rooms;

namespace Hipstr.Client.Commands
{
	public class SwitchToHistoryViewCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			((Frame)parameter).Navigate(typeof(RoomsView));
		}
	}
}