using System;
using Windows.UI.Xaml.Controls;

namespace Hipstr.Core.Services
{
	public interface INavigationService
	{
		void NavigateToPageOfType<T>();
		void NavigateToPageOfType(Type type);
		Type CurrentPageType { get; }
		void SetFrame(Frame frame);
	}
}