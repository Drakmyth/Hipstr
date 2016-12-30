using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatClient
	{
		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}