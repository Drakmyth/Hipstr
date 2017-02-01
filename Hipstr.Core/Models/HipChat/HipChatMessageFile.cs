using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageFile
	{
		[JsonProperty("url")]
		public Uri Uri { get; set; }

		[JsonProperty("thumb_url")]
		public Uri ThumbnailUri { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("size")]
		public int FileSize { get; set; }
	}
}