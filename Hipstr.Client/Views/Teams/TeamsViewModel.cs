using Hipstr.Core.Models;
using System.Collections.Generic;

namespace Hipstr.Client.Views.Teams
{
	public class TeamsViewModel
	{
		public List<Team> Teams { get; set; }

		public TeamsViewModel()
		{
			Teams = new List<Team> {
				new Team("Slalom PDP"),
				new Team("ExtraSpace Storage")
			};
		}
	}
}