using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatCreationResponse
	{
		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }
	}
}