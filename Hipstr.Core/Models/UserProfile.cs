using System;

namespace Hipstr.Core.Models
{
	public class UserProfile
	{
		public int Id { get; set; }
		public string Email { get; set; }
		public string MentionName { get; set; }
		public string Name { get; set; }
		public string Title { get; set; }
		public DateTime LastActive { get; set; }
		public DateTime Created { get; set; }
		public string PhotoUrl { get; set; }
		public bool IsGuest { get; set; }
		public bool IsGroupAdmin { get; set; }
		public bool IsDeleted { get; set; }
	}
}