using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessage
	{
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

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}