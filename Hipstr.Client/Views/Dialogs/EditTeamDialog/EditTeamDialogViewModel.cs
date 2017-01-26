using Hipstr.Client.Commands;
using JetBrains.Annotations;
using System;
using System.Windows.Input;

namespace Hipstr.Client.Views.Dialogs.EditTeamDialog
{
	[UsedImplicitly]
	public class EditTeamDialogViewModel : ViewModelBase
	{
		public event EventHandler<EditTeamDialogValidationEventArgs> Validation;
		public ICommand ValidateDataCommand { get; }

		private string _teamName;

		public string TeamName
		{
			get { return _teamName; }
			set
			{
				_teamName = value;
				OnPropertyChanged();
			}
		}

		private string _apiKey;

		public string ApiKey
		{
			get { return _apiKey; }
			set
			{
				_apiKey = value;
				OnPropertyChanged();
			}
		}

		private bool _isValidating;

		public bool IsValidating
		{
			get { return _isValidating; }
			private set
			{
				_isValidating = value;
				OnPropertyChanged();
			}
		}

		public EditTeamDialogViewModel()
		{
			_teamName = string.Empty;

			_isValidating = false;

			ValidateDataCommand = new RelayCommand(ValidateData, () => !IsValidating, this, nameof(IsValidating));
		}

		private void ValidateData()
		{
			IsValidating = true;
			ValidationResult teamNameValidation = ValidateTeamName(_teamName);
			IsValidating = false;

			Validation?.Invoke(this, new EditTeamDialogValidationEventArgs(teamNameValidation));
		}

		private static ValidationResult ValidateTeamName(string teamName)
		{
			return string.IsNullOrWhiteSpace(teamName) ? ValidationResult.Invalid("Team Name is required") : ValidationResult.Valid();
		}
	}
}