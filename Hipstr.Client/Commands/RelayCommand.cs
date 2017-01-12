using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action _execute;
		private readonly Func<bool> _canExecute;
		private readonly string[] _propertyNames;

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
			_propertyNames = new string[0];
		}

		public RelayCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(execute, canExecute)
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

		public void Execute(object parameter)
		{
			_execute?.Invoke();
		}
	}

	public class RelayCommand<TParameter> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action<TParameter> _execute;
		private readonly Func<TParameter, bool> _canExecute;
		private readonly string[] _propertyNames;

		public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(execute, canExecute)
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

		public void Execute(object parameter)
		{
			_execute?.Invoke((TParameter)parameter);
		}
	}
}