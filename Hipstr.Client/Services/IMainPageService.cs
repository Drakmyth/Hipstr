using System;

namespace Hipstr.Client.Services
{
	public interface IMainPageService
	{
		event EventHandler<string> TitleChanged;

		string Title { get; set; }
	}
}