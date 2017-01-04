using JetBrains.Annotations;
using Newtonsoft.Json;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatOAuthSession
	{
		[JsonProperty("scopes")]
		public string[] Scopes { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }

		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		[JsonProperty("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonProperty("client")]
		public HipChatOAuthClient Client { get; set; }

		[JsonProperty("owner")]
		public HipChatUser Owner { get; set; }

		[JsonProperty("owner_type")]
		public string OwnerType { get; set; }

		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("refresh_token")]
		public string RefreshToken { get; set; }
	}
}