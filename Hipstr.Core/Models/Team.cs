namespace Hipstr.Core.Models
{
	public class Team
	{
		public string Name { get; set; }

		public Team(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}