namespace Hipstr.Core.Models
{
	public class Room
	{
		public string Name { get; set; }

		public Room(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}