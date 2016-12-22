using Hipstr.Client.Commands;
using Hipstr.Core.Models;
using Hipstr.Core.Services;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml.Media.Imaging;

namespace Hipstr.Client.Views.Users
{
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
				_mainPageService.Title = _user.Name;
				OnPropertyChanged();
			}
		}

		private UserProfile _userProfile;

		public UserProfile UserProfile
		{
			get { return _userProfile; }
			set
			{
				_userProfile = value;
				OnPropertyChanged();
			}
		}

		private BitmapImage _profileImage;

		public BitmapImage ProfileImage
		{
			get { return _profileImage; }
			set
			{
				_profileImage = value;
				OnPropertyChanged();
			}
		}


		private readonly IHipChatService _hipChatService;
		private readonly IMainPageService _mainPageService;

		public UserProfileViewModel() : this(IoCContainer.Resolve<IHipChatService>(), IoCContainer.Resolve<IMainPageService>())
		{
		}

		public UserProfileViewModel(IHipChatService hipChatService, IMainPageService mainPageService)
		{
			_hipChatService = hipChatService;
			_mainPageService = mainPageService;

			_mainPageService.Title = "User Profile";

			ReloadUserProfileCommand = new RelayCommandAsync(ReloadUserProfileAsync);
		}

		private async Task ReloadUserProfileAsync()
		{
			UserProfile userProfile = await _hipChatService.GetUserProfileAsync(_user);
			UserProfile = userProfile;
			ProfileImage = new BitmapImage(new Uri(userProfile.PhotoUrl));
		}
	}
}