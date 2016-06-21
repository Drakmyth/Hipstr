using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class UserSummary
	{
		public int Id { get; set; }
		public Dictionary<string, Uri> Links { get; set; }
		public string MentionName { get; set; }
		public string Name { get; set; }
		public string Version { get; set; }
	}
}