using Newtonsoft.Json;
using System;
using JetBrains.Annotations;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatNotificationMessage
	{
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

//		[JsonProperty("message_links")]

		/*
		string url // The URL of the link that was extracted from the message
		string type // The type of link extracted which will determine the link metadata attributes
		*/

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}