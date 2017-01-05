﻿using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class RelayCommand : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Action _execute;
		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;

		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action execute, Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(execute, canExecute)
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
		private readonly string _propertyName;

		public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute = null)
		{
			_execute = execute;
			_canExecute = canExecute;
		}

		public RelayCommand(Action<TParameter> execute, Func<TParameter, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(execute, canExecute)
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
			return _canExecute?.Invoke((TParameter)parameter) ?? true;
		}

		public void Execute(object parameter)
		{
			_execute?.Invoke((TParameter)parameter);
		}
	}
}