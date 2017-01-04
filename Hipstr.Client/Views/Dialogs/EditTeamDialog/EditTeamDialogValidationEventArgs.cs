using Hipstr.Client.Views.Dialogs;

namespace Hipstr.Client.Events
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