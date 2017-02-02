using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatRoomCreationRequest
	{
		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("privacy")]
		public string Privacy { get; set; }

		[JsonProperty("delegate_admin_visibility")]
		public bool DelegateAdminVisibility { get; set; }

		[JsonProperty("topic")]
		public string Topic { get; set; }

		[JsonProperty("owner_user_id")]
		public string OwnerUserId { get; set; }

		[JsonProperty("guest_access")]
		public bool GuestAccess { get; set; }
	}
}