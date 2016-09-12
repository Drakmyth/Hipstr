using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class NavigateToViewCommand<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return App.Frame.CurrentSourcePageType != typeof(T);
		}

		public void Execute(object parameter)
		{
			App.Frame.Navigate(typeof(T), parameter);
		}
	}
}
