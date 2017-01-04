using Hipstr.Client.Commands;
using Hipstr.Client.Events;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
{
	[UsedImplicitly]
	public class AddTeamDialogViewModel : ViewModelBase
	{
		public event EventHandler<AddTeamDialogValidationEventArgs> Validation;
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
			set
			{
				_isValidating = value;
				OnPropertyChanged();
			}
		}

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;

		public AddTeamDialogViewModel(ITeamService teamService, IHipChatService hipChatService)
		{
			_teamService = teamService;
			_hipChatService = hipChatService;

			_teamName = string.Empty;
			_apiKey = string.Empty;

			_isValidating = false;

			ValidateDataCommand = new RelayCommandAsync(ValidateDataAsync, () => !IsValidating, this, nameof(IsValidating));
		}

		private async Task ValidateDataAsync()
		{
			IsValidating = true;
			ValidationResult teamNameValidation = ValidateTeamName(_teamName);
			ValidationResult apiKeyValidation = await ValidateApiKeyAsync(_apiKey);
			IsValidating = false;

			Validation?.Invoke(this, new AddTeamDialogValidationEventArgs(teamNameValidation, apiKeyValidation));
		}

		private static ValidationResult ValidateTeamName(string teamName)
		{
			return string.IsNullOrWhiteSpace(teamName) ? ValidationResult.Invalid("Team Name is required") : ValidationResult.Valid();
		}

		private async Task<ValidationResult> ValidateApiKeyAsync(string apiKey)
		{
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				return ValidationResult.Invalid("API Key is required");
			}

			IReadOnlyList<Team> teams = await _teamService.GetTeamsAsync();
			if (teams.Where(team => team.ApiKey == apiKey).Any())
			{
				return ValidationResult.Invalid("Provided API Key already registered");
			}

			try
			{
				await _hipChatService.GetApiKeyInfoAsync(apiKey);
			}
			catch
			{
				return ValidationResult.Invalid("Error validating provided API Key with HipChat server");
			}

			return ValidationResult.Valid();
		}
	}
}