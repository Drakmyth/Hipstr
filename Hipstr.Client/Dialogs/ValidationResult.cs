namespace Hipstr.Client.Dialogs
{
	public class ValidationResult
	{
		public bool IsValid { get; }
		public string Reason { get; }

		private ValidationResult(bool valid, string reason = "")
		{
			IsValid = valid;
			Reason = reason;
		}

		public static ValidationResult Valid()
		{
			return new ValidationResult(true);
		}

		public static ValidationResult Invalid(string reason)
		{
			return new ValidationResult(false, reason);
		}
	}
}