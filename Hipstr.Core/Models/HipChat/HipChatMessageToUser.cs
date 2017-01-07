using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageToUser
	{
		[JsonProperty("message")]
		public string Message { get; set; }

		[JsonProperty("notify")]
		public bool Notify { get; set; }

		[JsonProperty("message_format")]
		public string MessageFormat { get; set; }
	}
}