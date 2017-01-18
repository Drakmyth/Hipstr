using Newtonsoft.Json;
using System;

namespace Hipstr.Core.Models
{
	public class Emoticon
	{
		public int Id { get; set; }
		public string Shortcut { get; set; }
		public Uri Url { get; set; }

		[JsonIgnore]
		public Team Team { get; set; }
	}
}