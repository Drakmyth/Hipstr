using System;

namespace Hipstr.Core.Models
{
	public class MessageTwitterUser
	{
		public int Followers { get; set; }
		public string Name { get; set; }
		public Uri ProfileImageUri { get; set; }
		public string ScreenName { get; set; }
	}
}