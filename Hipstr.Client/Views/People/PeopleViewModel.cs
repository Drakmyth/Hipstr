using Hipstr.Core.Models;
using System.Collections.Generic;

namespace Hipstr.Client.Views.People
{
	public class PeopleViewModel
	{
		public List<Person> People { get; set; }

		public PeopleViewModel()
		{
			People = new List<Person> {
				new Person("Ian Burns"),
				new Person("Hayley Garment"),
				new Person("Brett Moquin")
			};
		}
	}
}