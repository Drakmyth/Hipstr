using System;
using System.ComponentModel;
using System.Windows.Input;

namespace Hipstr.Client.Commands
{
	public class NavigateToViewCommand<TView> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<bool> _canExecute;
		private readonly string _propertyName;

		public bool ClearBackStackOnNavigate { private get; set; }

		public NavigateToViewCommand(Func<bool> canExecute = null)
		{
			ClearBackStackOnNavigate = false;
			_canExecute = canExecute;
		}

		public NavigateToViewCommand(Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(canExecute)
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
			if (App.Frame.CurrentSourcePageType != typeof(TView))
			{
				App.Frame.Navigate(typeof(TView));
				if (ClearBackStackOnNavigate)
				{
					App.Frame.BackStack.Clear();
				}
			}
		}
	}

	public class NavigateToViewCommand<TView, TParameter> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<TParameter, bool> _canExecute;
		private readonly string _propertyName;

		public bool ClearBackStackOnNavigate { private get; set; }

		public NavigateToViewCommand(Func<TParameter, bool> canExecute = null)
		{
			ClearBackStackOnNavigate = false;
			_canExecute = canExecute;
		}

		public NavigateToViewCommand(Func<TParameter, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, string propertyName) : this(canExecute)
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
			if (App.Frame.CurrentSourcePageType != typeof(TView))
			{
				App.Frame.Navigate(typeof(TView), (TParameter)parameter);
				if (ClearBackStackOnNavigate)
				{
					App.Frame.BackStack.Clear();
				}
			}
		}
	}
}