using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatCollectionWrapper<TItem>
	{
		public IEnumerable<TItem> Items { get; set; }
		public Dictionary<string, Uri> Links { get; set; }
		public int MaxResults { get; set; }
		public int StartIndex { get; set; }
	}
}