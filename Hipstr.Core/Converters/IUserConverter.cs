using Hipstr.Core.Models;
using Hipstr.Core.Models.HipChat;

namespace Hipstr.Core.Converters
{
	public interface IUserConverter
	{
		User HipChatUserToUser(HipChatUser hcUser, Team team);
	}
}