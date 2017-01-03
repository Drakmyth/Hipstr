using Newtonsoft.Json;

namespace Hipstr.Core.Models
{
	public class Room
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public bool IsArchived { get; set; }
		public string Privacy { get; set; }

		[JsonIgnore]
		public Team Team { get; set; }
	}
}