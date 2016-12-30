using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatUser
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("mention_name")]
		public string MentionName { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }
	}
}