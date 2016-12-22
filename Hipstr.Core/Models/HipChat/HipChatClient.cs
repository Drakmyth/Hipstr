using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatClient
	{
		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}