using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Animation;

namespace Hipstr.Client.Commands
{
	public class NavigateToViewCommand<TView> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<bool> _canExecute;
		private readonly string[] _propertyNames;

		public bool ClearBackStackOnNavigate { private get; set; }
		public Type BackToSpecificType { private get; set; }

		public NavigateToViewCommand(Func<bool> canExecute = null)
		{
			ClearBackStackOnNavigate = false;
			BackToSpecificType = null;
			_canExecute = canExecute;
			_propertyNames = new string[0];
		}

		public NavigateToViewCommand(Func<bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(canExecute)
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
			if (App.Frame.CurrentSourcePageType == typeof(TView)) return;

			if (BackToSpecificType != null)
			{
				App.Frame.Navigate(BackToSpecificType, null, new SuppressNavigationTransitionInfo());
			}

			if (ClearBackStackOnNavigate)
			{
				App.Frame.BackStack.Clear();
			}

			App.Frame.Navigate(typeof(TView));
		}
	}

	public class NavigateToViewCommand<TView, TParameter> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly Func<TParameter, bool> _canExecute;
		private readonly string[] _propertyNames;

		public bool ClearBackStackOnNavigate { private get; set; }
		public Type BackToSpecificType { private get; set; }

		public NavigateToViewCommand(Func<TParameter, bool> canExecute = null)
		{
			ClearBackStackOnNavigate = false;
			BackToSpecificType = null;
			_canExecute = canExecute;
			_propertyNames = new string[0];
		}

		public NavigateToViewCommand(Func<TParameter, bool> canExecute, INotifyPropertyChanged propertyChangedNotifier, params string[] propertyNames) : this(canExecute)
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
			if (App.Frame.CurrentSourcePageType == typeof(TView)) return;

			if (BackToSpecificType != null)
			{
				App.Frame.Navigate(BackToSpecificType, parameter, new SuppressNavigationTransitionInfo());
			}

			if (ClearBackStackOnNavigate)
			{
				App.Frame.BackStack.Clear();
			}

			App.Frame.Navigate(typeof(TView), (TParameter)parameter);
		}
	}
}