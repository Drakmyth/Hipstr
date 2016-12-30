using System;

namespace Hipstr.Client
{
	public interface IMainPageService
	{
		event EventHandler<string> TitleChanged;

		string Title { get; set; }
	}
}