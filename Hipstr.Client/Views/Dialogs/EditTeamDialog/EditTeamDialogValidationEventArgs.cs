namespace Hipstr.Client.Views.Dialogs.EditTeamDialog
{
	public class EditTeamDialogValidationEventArgs
	{
		public ValidationResult TeamNameValidationResult { get; }

		public EditTeamDialogValidationEventArgs(ValidationResult teamNameValidationResult)
		{
			TeamNameValidationResult = teamNameValidationResult;
		}
	}
}