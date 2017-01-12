using System;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommandAsync : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<Task> _execute;
		private readonly Func<bool> _canExecute;
		private readonly string[] _propertyNames;

		public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
			_propertyNames = new string[0];
		}

		public RelayCommandAsync(Func<Task> execute, Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(execute, canExecute)
		{
			_propertyNames = propertyNames;
			propertyChangedNotifier.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!_propertyNames.Contains(e.PropertyName)) return;

			CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke() ?? true;
		}

		public async void Execute(object parameter)
		{
			// ReSharper disable once PossibleNullReferenceException
			await _execute?.Invoke();
		}
	}

	public class RelayCommandAsync<TParameter> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<TParameter, Task> _execute;
		private readonly Func<TParameter, bool> _canExecute;
		private readonly string[] _propertyNames;

		public RelayCommandAsync(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
			_propertyNames = new string[0];
		}

		public RelayCommandAsync(Func<TParameter, Task> execute, Func<TParameter, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(execute, canExecute)
		{
			_propertyNames = propertyNames;
			propertyChangedNotifier.PropertyChanged += OnPropertyChanged;
		}

		private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (!_propertyNames.Contains(e.PropertyName)) return;

			CanExecuteChanged?.Invoke(sender, EventArgs.Empty);
		}

		public bool CanExecute(object parameter)
		{
			return _canExecute?.Invoke((TParameter)parameter) ?? true;
		}

		public async void Execute(object parameter)
		{
			// ReSharper disable once PossibleNullReferenceException
			await _execute?.Invoke((TParameter)parameter);
		}
	}
}