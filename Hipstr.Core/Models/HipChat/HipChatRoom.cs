using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class HipChatRoom
	{
		public int Id { get; set; }
		public bool IsArchived { get; set; }
		public Dictionary<string, Uri> Links { get; set; }
		public string Name { get; set; }
		public string Privacy { get; set; }
		public string Version { get; set; }
	}
}