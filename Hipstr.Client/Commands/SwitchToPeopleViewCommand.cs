using Hipstr.Client.Views.People;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Client.Commands
{
	public class SwitchToPeopleViewCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			if (parameter == null) return true;

			return ((Frame)parameter).CurrentSourcePageType != typeof(PeopleView);
		}

		public void Execute(object parameter)
		{
			((Frame)parameter).Navigate(typeof(PeopleView));
		}
	}
}