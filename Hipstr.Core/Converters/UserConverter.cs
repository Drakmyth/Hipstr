using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;

namespace Hipstr.Core.Converters
{
	public class UserConverter : IUserConverter
	{
		public User HipChatUserToUser(HipChatUser hcUser, Team team)
		{
			return new User
			{
				Id = hcUser.Id,
				Handle = hcUser.MentionName,
				Name = hcUser.Name,
				Team = team
			};
		}
	}
}