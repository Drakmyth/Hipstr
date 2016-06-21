using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class AddTeamCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			throw new NotImplementedException();
		}

		public void Execute(object parameter)
		{
			throw new NotImplementedException();
		}
	}
}
