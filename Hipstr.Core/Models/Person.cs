namespace Hipstr.Core.Models
{
	public class Person
	{
		public string Name { get; set; }

		public Person(string name)
		{
			Name = name;
		}

		public override string ToString()
		{
			return Name;
		}
	}
}