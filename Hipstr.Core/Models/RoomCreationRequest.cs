namespace Hipstr.Core.Models
{
	public class RoomCreationRequest
	{
		public Team Team { get; set; }
		public string Name { get; set; }
		public string Privacy { get; set; }
		public bool? DelegateAdminVisibility { get; set; }
		public string Topic { get; set; }
		public bool GuestAccess { get; set; }
	}
}