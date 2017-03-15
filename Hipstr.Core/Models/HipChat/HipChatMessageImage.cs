using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatMessageImage
	{
		[JsonProperty("image")]
		public Uri ImageUri { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
	}
}