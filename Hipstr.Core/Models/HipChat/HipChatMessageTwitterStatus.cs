using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageTwitterStatus
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("created")]
		public string Created { get; set; }

		[JsonProperty("text")]
		public string Text { get; set; }

		[JsonProperty("profileImageUrl")]
		public Uri ProfileImageUri { get; set; }

		[JsonProperty("source")]
		public string Source { get; set; }

		[JsonProperty("screenName")]
		public string ScreenName { get; set; }
	}
}