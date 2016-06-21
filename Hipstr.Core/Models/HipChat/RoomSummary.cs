using System;
using System.Collections.Generic;

namespace Hipstr.Core.Models.HipChat
{
	public class RoomSummary
	{
		public int Id { get; set; }
		public bool IsArchived { get; set; }
		public Dictionary<string, Uri> Links { get; set; }
		public string Name { get; set; }
		public string Privacy { get; set; }
		public string Version { get; set; }

		// TODO: Replace with DataTemplate in View
		public override string ToString()
		{
			return Name;
		}
	}
}