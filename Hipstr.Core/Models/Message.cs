using System;

namespace Hipstr.Core.Models
{
	public class Message
	{
		public User PostedBy { get; set; }
		public DateTime Date { get; set; }
		public string Text { get; set; }
	}
}