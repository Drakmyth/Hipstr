using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatOAuthClient
	{
		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("room")]
		public HipChatRoom Room { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("allowed_scopes")]
		public string[] AllowedScopes { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}