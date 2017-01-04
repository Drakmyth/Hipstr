using System.Collections.Generic;

namespace Hipstr.Core.Models
{
	public class ApiKeyInfo
	{
		public IReadOnlyList<string> Scopes { get; set; }
		public string ApiKey { get; set; }
		public int Id { get; set; }
		public string RefreshToken { get; set; }
	}
}