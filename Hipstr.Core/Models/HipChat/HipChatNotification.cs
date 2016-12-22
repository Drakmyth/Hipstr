using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatNotification
	{
		[JsonProperty("attach_to")]
		public string AttachTo { get; set; }

		[JsonProperty("card")]
		public string Card { get; set; }

		[JsonProperty("color")]
		public string Color { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("from")]
		public string From { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("mentions")]
		public HipChatUser[] Mentions { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("message_format")]
		public string MessageFormat { get; set; }

		[JsonProperty("notification_sender")]
		public HipChatNotificationSender NotificationSender { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}