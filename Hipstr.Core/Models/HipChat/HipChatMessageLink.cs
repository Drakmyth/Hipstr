using JetBrains.Annotations;
using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatMessageLink
	{
		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("title")]
		public string Title { get; set; }

		[JsonProperty("headerText")]
		public string HeaderText { get; set; }

		[JsonProperty("linkText")]
		public string LinkText { get; set; }

		[JsonProperty("faviconUrl")]
		public Uri FaviconUri { get; set; }

		[JsonProperty("fullUrl")]
		public Uri FullUri { get; set; }
	}
}