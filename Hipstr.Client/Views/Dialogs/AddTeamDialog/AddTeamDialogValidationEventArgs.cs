namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
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