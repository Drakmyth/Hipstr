using Hipstr.Core.Messaging;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System;
using Windows.UI.Xaml.Data;

namespace Hipstr.Client.Converters
{
	public class RoomToMessageSourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var room = value as Room;
			return room == null ? null : new RoomMessageSource(IoCContainer.Resolve<IHipChatService>(), room);
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var messageSource = value as RoomMessageSource;
			return messageSource?.Room;
		}
	}
}