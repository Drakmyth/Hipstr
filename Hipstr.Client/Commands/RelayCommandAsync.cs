using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommandAsync : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;

		public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke() ?? true;
		}

		public async void Execute(object parameter)
		{
			await _execute?.Invoke();
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}

	public class RelayCommandAsync<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<T, Task> _execute;
		private readonly Func<T, bool> _canExecute;

		public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

		public async void Execute(object parameter)
		{
			await _execute?.Invoke((T)parameter);
		}

		public void RaiseCanExecuteChanged()
		{
			CanExecuteChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}