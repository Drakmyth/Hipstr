using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageImage
	{
		[JsonProperty("image")]
		public Uri ImageUri { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}