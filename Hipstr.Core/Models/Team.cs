namespace Hipstr.Core.Models
{
	public class Team
	{
		public string Name { get; set; }
		public string ApiKey { get; set; }

		public Team(string name, string apiKey)
		{
			Name = name;
			ApiKey = apiKey;
		}
	}
}