using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hipstr.Client.Views.Dialogs.AddRoomDialog
{
	[UsedImplicitly]
	public class AddRoomDialogViewModel : ViewModelBase
	{
		public event EventHandler<AddRoomDialogValidationEventArgs> Validation;
		public ICommand ValidateDataCommand { get; }

		private ObservableCollection<Team> _teams;

		public ObservableCollection<Team> Teams
		{
			get { return _teams; }
			set
			{
				_teams = value;
				OnPropertyChanged();
			}
		}

		private Team _selectedTeam;

		public Team SelectedTeam
		{
			get { return _selectedTeam; }
			set
			{
				_selectedTeam = value;
				OnPropertyChanged();
			}
		}

		private string _name;

		public string Name
		{
			get { return _name; }
			set
			{
				_name = value;
				OnPropertyChanged();
			}
		}

		private string _topic;

		public string Topic
		{
			get { return _topic; }
			set
			{
				_topic = value;
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

		private bool _isVisibleToDelegateAdmin;

		public bool IsVisibleToDelegateAdmin
		{
			get { return _isVisibleToDelegateAdmin; }
			set
			{
				_isVisibleToDelegateAdmin = value;
				OnPropertyChanged();
			}
		}

		private bool _isInvisibleToDelegateAdmin;

		public bool IsInvisibleToDelegateAdmin
		{
			get { return _isInvisibleToDelegateAdmin; }
			set
			{
				_isInvisibleToDelegateAdmin = value;
				OnPropertyChanged();
			}
		}

		private bool _isDelegateAdminVisibilityGroupDefault;

		public bool IsDelegateAdminVisibilityGroupDefault
		{
			get { return _isDelegateAdminVisibilityGroupDefault; }
			set
			{
				_isDelegateAdminVisibilityGroupDefault = value;
				OnPropertyChanged();
			}
		}

		private bool _isPrivate;

		public bool IsPrivate
		{
			get { return _isPrivate; }
			set
			{
				_isPrivate = value;
				OnPropertyChanged();
			}
		}

		private bool guestAccessEnabled;

		public bool GuestAccessEnabled
		{
			get { return guestAccessEnabled; }
			set
			{
				guestAccessEnabled = value;
				OnPropertyChanged();
			}
		}

		private IHipChatService _hipChatService;

		public AddRoomDialogViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;

			_teams = new ObservableCollection<Team>();
			_name = string.Empty;
			_topic = string.Empty;

			_isValidating = false;

			_isVisibleToDelegateAdmin = false;
			_isInvisibleToDelegateAdmin = false;
			_isDelegateAdminVisibilityGroupDefault = true;

			ValidateDataCommand = new RelayCommandAsync(ValidateDataAsync, () => !IsValidating, this, nameof(IsValidating));
		}

		private async Task ValidateDataAsync()
		{
			IsValidating = true;
			ValidationResult nameValidation = await ValidateName(_hipChatService, _name, SelectedTeam);
			IsValidating = false;

			Validation?.Invoke(this, new AddRoomDialogValidationEventArgs(nameValidation));
		}

		private static async Task<ValidationResult> ValidateName(IHipChatService hipChatService, string teamName, Team team)
		{
			const int nameMinLength = 1;
			const int nameMaxLength = 50;

			if (string.IsNullOrWhiteSpace(teamName))
			{
				return ValidationResult.Invalid("Name is required");
			}

			if (teamName.Length < nameMinLength || teamName.Length > nameMaxLength)
			{
				return ValidationResult.Invalid($"Name length must be between {nameMinLength} and {nameMaxLength} characters");
			}

			IReadOnlyList<Room> existingRooms = await hipChatService.GetRoomsForTeamAsync(team, HipChatCacheBehavior.RefreshCache);

			if (existingRooms.Where(room => room.Name == teamName).SingleOrDefault() != null)
			{
				return ValidationResult.Invalid("Room with that name already exists");
			}

			return ValidationResult.Valid();
		}
	}
}