using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Hipstr.Core.Comparers
{
	public class RoomNameComparer : IComparer<string>
	{
		private static RoomNameComparer _instance;
		public static RoomNameComparer Instance => _instance ?? (_instance = new RoomNameComparer());

		public int Compare(string x, string y)
		{
			var alphanumRegex = new Regex("^[A-Za-z0-9]$");

			string xGroup = x[0].ToString();
			string yGroup = y[0].ToString();

			bool xIsAlphanumeric = alphanumRegex.IsMatch(xGroup);
			bool yIsAlphanumeric = alphanumRegex.IsMatch(yGroup);

			if (xIsAlphanumeric == yIsAlphanumeric)
			{
				return string.CompareOrdinal(x.ToUpper(), y.ToUpper());
			}

			return xIsAlphanumeric ? -1 : 1;
		}
	}
}