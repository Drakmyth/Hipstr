using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageVideo
	{
		[JsonProperty("thumbnailUrl")]
		public Uri ThumbnailUri { get; set; }

		[JsonProperty("views")]
		public int Views { get; set; }

		[JsonProperty("author")]
		public string Author { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }
	}
}