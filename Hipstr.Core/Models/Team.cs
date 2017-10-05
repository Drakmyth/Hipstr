namespace Hipstr.Core.Models
{
	public class Team
	{
		public string Name { get; set; }
		public string ApiKey { get; }

		public static bool operator ==(Team left, Team right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(Team left, Team right)
		{
			return !Equals(left, right);
		}

		public Team(string name, string apiKey)
		{
			Name = name;
			ApiKey = apiKey;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;

			return string.Equals(ApiKey, ((Team)obj).ApiKey);
		}

		public override int GetHashCode()
		{
			return ApiKey != null ? ApiKey.GetHashCode() : 0;
		}
	}
}