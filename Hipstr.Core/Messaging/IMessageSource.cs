using Hipstr.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Hipstr.Core.Messaging
{
	public interface IMessageSource
	{
		string Name { get; }

		void SendMessage();
		Task<IReadOnlyList<Message>> GetMessagesAsync();
	}
}