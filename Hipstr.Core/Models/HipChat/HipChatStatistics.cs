using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatStatistics
	{
		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }
	}
}