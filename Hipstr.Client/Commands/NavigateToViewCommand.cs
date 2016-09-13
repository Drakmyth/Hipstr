using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class NavigateToViewCommand<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			if (App.Frame.CurrentSourcePageType != typeof(T))
			{
				App.Frame.Navigate(typeof(T), parameter);
			}
		}
	}
}
