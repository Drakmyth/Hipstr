using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatRoom
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("is_archived")]
		public bool IsArchived { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("privacy")]
		public string Privacy { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }
	}
}