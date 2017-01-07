using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatMessageToRoom
	{
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}