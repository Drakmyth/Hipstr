using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageTwitterUser
	{
		[JsonProperty("followers")]
		public int Followers { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("profileImageUrl")]
		public Uri ProfileImageUri { get; set; }

		[JsonProperty("screenName")]
		public string ScreenName { get; set; }
	}
}