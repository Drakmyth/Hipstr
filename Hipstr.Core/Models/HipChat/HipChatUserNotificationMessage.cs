using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatUserNotificationMessage
	{
		[JsonProperty("attach_to")]
		public string AttachTo { get; set; }

		[JsonProperty("date")]
		public DateTime Date { get; set; }

		[JsonProperty("from")]
		public HipChatUser From { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("mentions")]
		public HipChatUser[] Mentions { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("message_format")]
		public string MessageFormat { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}