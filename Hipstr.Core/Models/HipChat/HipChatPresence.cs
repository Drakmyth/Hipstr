using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatPresence
	{
		[JsonProperty("status")]
		public string Status { get; set; }

		[JsonProperty("idle")]
		public int Idle { get; set; }

		[JsonProperty("show")]
		public string Show { get; set; }

		[JsonProperty("client")]
		public HipChatClient Client { get; set; }

		[JsonProperty("is_online")]
		public bool IsOnline { get; set; }
	}
}