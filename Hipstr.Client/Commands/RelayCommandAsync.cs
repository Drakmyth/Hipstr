using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommandAsync : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;

		public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(execute, canExecute)
		{
			_propertyName = propertyName;
			propertyChangedNotifier.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != _propertyName) return;

			CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke() ?? true;
		}

		public async void Execute(object parameter)
		{
			await _execute?.Invoke();
		}
	}

	public class RelayCommandAsync<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<T, Task> _execute;
		private readonly Func<T, bool> _canExecute;
		private readonly string _propertyName;

		public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommandAsync(Func<T, Task> execute, Func<T, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(execute, canExecute)
		{
			_propertyName = propertyName;
			propertyChangedNotifier.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName != _propertyName) return;

			CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((T)parameter) ?? true;
		}

		public async void Execute(object parameter)
		{
			await _execute?.Invoke((T)parameter);
		}
	}
}