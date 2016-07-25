using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public interface IRoomReloader
	{
		void ReloadRoom();
	}

	public class ReloadRoomCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly IRoomReloader _reloader;

		public ReloadRoomCommand(IRoomReloader reloader)
		{
			_reloader = reloader;
		}

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			_reloader.ReloadRoom();
		}
	}
}