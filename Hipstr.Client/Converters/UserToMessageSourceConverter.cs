using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class UserToMessageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var user = value as User;
			return user == null ? null : new UserMessageSource(IoCContainer.Resolve<IHipChatService>(), user);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var messageSource = value as UserMessageSource;
			return messageSource?.User;
		}
	}
}