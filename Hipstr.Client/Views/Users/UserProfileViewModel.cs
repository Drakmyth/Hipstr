using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using JetBrains.Annotations;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Hipstr.Client.Views.Users
{
	[UsedImplicitly]
	public class UserProfileViewModel : ViewModelBase
	{
		public ICommand ReloadUserProfileCommand { get; }

		private User _user;

		public User User
		{
			get { return _user; }
			set
			{
				_user = value;
				OnPropertyChanged();
			}
		}

		private UserProfile _userProfile;

		public UserProfile UserProfile
		{
			get { return _userProfile; }
			private set
			{
				_userProfile = value;
				OnPropertyChanged();
			}
		}

		private BitmapImage _profileImage;

		public BitmapImage ProfileImage
		{
			get { return _profileImage; }
			private set
			{
				_profileImage = value;
				OnPropertyChanged();
			}
		}

		private bool _loadingUserProfile;

		public bool LoadingUserProfile
		{
			get { return _loadingUserProfile; }
			private set
			{
				_loadingUserProfile = value;
				OnPropertyChanged();
			}
		}

		private readonly IHipChatService _hipChatService;

		public UserProfileViewModel(IHipChatService hipChatService)
		{
			_hipChatService = hipChatService;
			_loadingUserProfile = false;

			ReloadUserProfileCommand = new RelayCommandAsync(ReloadUserProfileAsync, () => !LoadingUserProfile, this, nameof(LoadingUserProfile));
		}

		public override async Task InitializeAsync()
		{
			await ReloadUserProfileAsync();
		}

		private async Task ReloadUserProfileAsync()
		{
			try
			{
				LoadingUserProfile = true;
				UserProfile userProfile = await _hipChatService.GetUserProfileAsync(_user);
				UserProfile = userProfile;
				ProfileImage = new BitmapImage(new Uri(userProfile.PhotoUrl))
				{
					DecodePixelHeight = 180,
					DecodePixelWidth = 180
				};
			}
			finally
			{
				LoadingUserProfile = false;
			}
		}
	}
}