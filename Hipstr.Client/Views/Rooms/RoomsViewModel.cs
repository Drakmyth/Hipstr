using Hipstr.Core.Models;
using System.Collections.Generic;

namespace Hipstr.Client.Views.Rooms
{
	public class RoomsViewModel
	{
		public List<Room> Rooms { get; set; }

		public RoomsViewModel()
		{
			Rooms = new List<Room> {
				new Room("Extraspace - Sonics"),
				new Room("DCW Watercooler")
			};
		}
	}
}