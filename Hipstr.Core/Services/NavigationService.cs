using System;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Core.Services
{
	public class NavigationService : INavigationService
	{
		private Frame _frame;
		private Frame Frame
		{
			get
			{
				if (_frame == null) { throw new InvalidOperationException("NavigationService Frame not initialized."); }
				return _frame;
			}
		}

		public Type CurrentPageType => Frame.CurrentSourcePageType;

		public void SetFrame(Frame frame)
		{
			_frame = frame;
		}

		public void NavigateToPageOfType<T>()
		{
			NavigateToPageOfType(typeof(T));
		}

		public void NavigateToPageOfType(Type type)
		{
			Frame.Navigate(type);
		}
	}
}