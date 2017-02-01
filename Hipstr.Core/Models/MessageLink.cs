using System;

namespace Hipstr.Core.Models
{
	public class MessageLink
	{
		public string Description { get; set; }
		public string Title { get; set; }
		public string HeaderText { get; set; }
		public string LinkText { get; set; }
		public Uri FaviconUri { get; set; }
		public Uri FullUri { get; set; }
	}
}