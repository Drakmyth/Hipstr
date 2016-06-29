using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatNotification
	{
		public string AttachTo { get; set; }
		public string Card { get; set; }
		public string Color { get; set; }
		public DateTime Date { get; set; }
		public string From { get; set; }
		public string Id { get; set; }
		public HipChatUser[] Mentions { get; set; }
		public string Message { get; set; }
		public string MessageFormat { get; set; }
		public HipChatNotificationSender NotificationSender { get; set; }
		public string Type { get; set; }
	}
}