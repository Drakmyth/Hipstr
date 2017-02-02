namespace Hipstr.Client.Views.Dialogs.AddRoomDialog
{
	public class AddRoomDialogValidationEventArgs
	{
		public ValidationResult NameValidationResult { get; }

		public AddRoomDialogValidationEventArgs(ValidationResult nameValidationResult)
		{
			NameValidationResult = nameValidationResult;
		}
	}
}