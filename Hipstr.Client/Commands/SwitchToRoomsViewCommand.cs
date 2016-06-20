using Hipstr.Client.Views.Rooms;
using System;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using Hipstr.Core.Services;

namespace Hipstr.Client.Commands
{
	public class SwitchToRoomsViewCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			if (parameter == null) return true;

			return ((Frame)parameter).CurrentSourcePageType != typeof(RoomsView);
		}

		public void Execute(object parameter)
		{
			((Frame)parameter).Navigate(typeof(RoomsView));
		}
	}
}