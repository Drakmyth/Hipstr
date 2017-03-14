using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatNotificationSender
	{
		[JsonProperty("client_id")]
		public string ClientId { get; set; }

		[JsonProperty("id")]
		public string Id { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }
	}
}