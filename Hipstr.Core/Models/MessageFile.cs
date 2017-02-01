using System;

namespace Hipstr.Core.Models
{
	public class MessageFile
	{
		public Uri Uri { get; set; }
		public Uri ThumbnailUri { get; set; }
		public string Name { get; set; }
		public int FileSize { get; set; }
	}
}