using Hipstr.Core.Services;
using System;
using System.Windows.Input;

namespace Hipstr.Client.Commands.ViewCommands
{
	public abstract class NavigateToViewCommand<T> : ICommand
	{
		public event EventHandler CanExecuteChanged;

		private readonly INavigationService _navigationService;

		protected NavigateToViewCommand() : this(IoCContainer.Resolve<INavigationService>()) { }
		private NavigateToViewCommand(INavigationService navigationService)
		{
			_navigationService = navigationService;
		}

		public virtual bool CanExecute(object parameter)
		{
			return _navigationService.CurrentPageType != typeof(T);
		}

		public virtual void Execute(object parameter)
		{
			_navigationService.NavigateToPageOfType<T>(parameter);
		}
	}
}
