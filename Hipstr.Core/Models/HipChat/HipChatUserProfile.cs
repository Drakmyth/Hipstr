using Newtonsoft.Json;
using System;
using JetBrains.Annotations;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatUserProfile
	{
		[JsonProperty("xmpp_jid")]
		public string XmppJid { get; set; }

		[JsonProperty("is_deleted")]
		public bool IsDeleted { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("last_active")]
		public string LastActive { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("presence")]
		public HipChatPresence Presence { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("mention_name")]
		public string MentionName { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("roles")]
		public string[] Roles { get; set; }

		[JsonProperty("is_group_admin")]
		public bool IsGroupAdmin { get; set; }

		[JsonProperty("timezone")]
		public string Timezone { get; set; }

		[JsonProperty("is_guest")]
		public bool IsGuest { get; set; }

		[JsonProperty("email")]
		public string Email { get; set; }

		[JsonProperty("photo_url")]
		public string PhotoUrl { get; set; }
	}
}