using System;

namespace Hipstr.Core.Models
{
	public class MessageTwitterStatus
	{
		public string Name { get; set; }
		public string Created { get; set; }
		public string Text { get; set; }
		public Uri ProfileImageUri { get; set; }
		public string Source { get; set; }
		public string ScreenName { get; set; }
	}
}