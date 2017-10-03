using Hipstr.Core.Messaging;
using System;
using System.Linq;
using System.Text;

namespace Hipstr.Client.Utility
{
	public static class GroupNameSelectors
	{
		public static string StandardGroupNameSelector(IMessageSource s)
		{
			string name = s.Name.Normalize(NormalizationForm.FormD);
			char key = name.FirstOrDefault(char.IsLetterOrDigit);

			if (!char.IsLetterOrDigit(key))
			{
				return "\U0001F310";
			}

			return char.IsLetter(key) ? char.ToUpper(key).ToString() : "#";
		}
	}
}