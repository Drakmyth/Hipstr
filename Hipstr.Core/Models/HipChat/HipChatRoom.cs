using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatRoom
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("xmpp_jid")]
		public string JabberId { get; set; }

		[JsonProperty("statistics")]
		public HipChatStatistics Statistics { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("created")]
		public DateTime Created { get; set; }

		[JsonProperty("is_archived")]
		public bool IsArchived { get; set; }

		[JsonProperty("privacy")]
		public string Privacy { get; set; }

		[JsonProperty("is_guest_accessible")]
		public bool IsGuestAccessible { get; set; }

		[JsonProperty("avatar_url")]
		public Uri AvatarUri { get; set; }

		[JsonProperty("delegate_admin_visibility")]
		public bool? DelegateAdminVisibility { get; set; }

		[JsonProperty("topic")]
		public string Topic { get; set; }

		[JsonProperty("version")]
		public string Version { get; set; }

		[JsonProperty("owner")]
		public HipChatUser Owner { get; set; }

		[JsonProperty("guest_access_url")]
		public Uri GuestAccessUri { get; set; }
	}
}