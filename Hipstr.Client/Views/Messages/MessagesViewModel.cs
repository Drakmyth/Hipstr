using Hipstr.Core.Models;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views.Messages
{
	public class MessagesViewModel
	{
		public ObservableCollection<Message> Messages { get; set; }
	}
}