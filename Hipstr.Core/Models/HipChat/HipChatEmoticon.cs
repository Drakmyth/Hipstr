﻿using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	[UsedImplicitly]
	public class HipChatEmoticon
	{
		[JsonProperty("id")]
		public int Id { get; set; }

		[JsonProperty("links")]
		public Dictionary<string, Uri> Links { get; set; }

		[JsonProperty("shortcut")]
		public string Shortcut { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("url")]
		public Uri Url { get; set; }

		[JsonProperty("height")]
		public int Height { get; set; }

		[JsonProperty("width")]
		public int Width { get; set; }

		[JsonProperty("creator")]
		public HipChatUser Creator { get; set; }
	}
}