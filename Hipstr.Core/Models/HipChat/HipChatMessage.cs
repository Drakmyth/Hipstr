﻿using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
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

		[JsonProperty("message_links")]
		public IEnumerable<object> MessageLinks { get; set; }

		[JsonProperty("file")]
		public HipChatMessageFile File { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}