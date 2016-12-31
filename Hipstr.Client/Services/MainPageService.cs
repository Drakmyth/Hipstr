﻿using System;
using JetBrains.Annotations;

namespace Hipstr.Client.Services
{
	[UsedImplicitly]
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