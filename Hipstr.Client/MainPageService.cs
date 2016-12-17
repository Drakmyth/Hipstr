using System;

namespace Hipstr.Client
{
	public interface IMainPageService
	{
		event EventHandler<string> TitleChanged;

		string Title { get; set; }
	}

	public class MainPageService : IMainPageService
	{
		public event EventHandler<string> TitleChanged;

		private string _title;

		public string Title
		{
			get { return _title; }
			set
			{
				_title = value;
				TitleChanged?.Invoke(this, _title);
			}
		}
	}
}