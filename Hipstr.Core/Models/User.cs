﻿using Newtonsoft.Json;

namespace Hipstr.Core.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string Handle { get; set; }

		[JsonIgnore]
		public Team Team { get; set; }
	}
}