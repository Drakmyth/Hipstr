using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action _execute;
		private readonly Func<bool> _canExecute;

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke() ?? true;
		}

		public void Execute(object parameter)
		{
			_execute?.Invoke();
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public class RelayCommand<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action<T> _execute;
		private readonly Func<T, bool> _canExecute;

		public RelayCommand(Action<T> execute, Func<T, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute?.Invoke((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}