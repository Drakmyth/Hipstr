using System;

namespace Hipstr.Core.Models
{
	public class MessageVideo
	{
		public Uri ThumbnailUri { get; set; }
		public int Views { get; set; }
		public string Author { get; set; }
		public string Title { get; set; }
	}
}