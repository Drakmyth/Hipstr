using Hipstr.Client.Views.Teams;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Commands
{
	public class SwitchToTeamsViewCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			if (parameter == null) return true;

			return ((Frame)parameter).CurrentSourcePageType != typeof(TeamsView);
		}

		public void Execute(object parameter)
		{
			((Frame)parameter).Navigate(typeof(TeamsView));
		}
	}
}