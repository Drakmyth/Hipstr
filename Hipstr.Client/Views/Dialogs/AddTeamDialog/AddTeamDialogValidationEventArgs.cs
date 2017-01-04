using Hipstr.Client.Views.Dialogs;

namespace Hipstr.Client.Events
{
	public class AddTeamDialogValidationEventArgs
	{
		public ValidationResult TeamNameValidationResult { get; }
		public ValidationResult ApiKeyValidationResult { get; }

		public AddTeamDialogValidationEventArgs(ValidationResult teamNameValidationResult, ValidationResult apiKeyValidationResult)
		{
			TeamNameValidationResult = teamNameValidationResult;
			ApiKeyValidationResult = apiKeyValidationResult;
		}
	}
}