﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatCollectionWrapper<TItem>
	{
		[JsonProperty("items")]
		public IEnumerable<TItem> Items { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("max_results")]
		public int MaxResults { get; set; }

		[JsonProperty("start_index")]
		public int StartIndex { get; set; }
	}
}