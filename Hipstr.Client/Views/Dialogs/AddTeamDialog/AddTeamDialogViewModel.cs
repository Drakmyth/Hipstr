using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Dialogs.AddTeamDialog
{
	public class AddTeamDialogViewModel : ViewModelBase
	{
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

		private ValidationResult _teamNameValidation;

		public ValidationResult TeamNameValidation
		{
			get { return _teamNameValidation; }
			private set
			{
				_teamNameValidation = value;
				OnPropertyChanged();
			}
		}

		private ValidationResult _apiKeyValidation;

		public ValidationResult ApiKeyValidation
		{
			get { return _apiKeyValidation; }
			private set
			{
				_apiKeyValidation = value;
				OnPropertyChanged();
			}
		}

		private readonly ITeamService _teamService;
		private readonly IHipChatService _hipChatService;

		public AddTeamDialogViewModel(ITeamService teamService, IHipChatService hipChatService)
		{
			_teamService = teamService;
			_hipChatService = hipChatService;

			_teamNameValidation = ValidationResult.Valid();
			_apiKeyValidation = ValidationResult.Valid();

			ValidateDataCommand = new RelayCommandAsync(ValidateDataAsync);
		}

		private async Task ValidateDataAsync()
		{
			ValidationResult teamNameValidation = ValidateTeamName(_teamName);
			ValidationResult apiKeyValidation = await ValidateApiKeyAsync(_apiKey);

			TeamNameValidation = teamNameValidation;
			ApiKeyValidation = apiKeyValidation;

			UpdateVisualStates();

			if (!teamNameValidation.IsValid || !apiKeyValidation.IsValid) return;

			Hide();
			_taskCompletionSource.SetResult(new DialogResult<Team>(new Team(TeamName, ApiKey)));
		}

		private static ValidationResult ValidateTeamName(string teamName)
		{
			return string.IsNullOrWhiteSpace(teamName) ? ValidationResult.Invalid("Team Name is required") : ValidationResult.Valid();
		}

		private async Task<ValidationResult> ValidateApiKeyAsync(string apiKey)
		{
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