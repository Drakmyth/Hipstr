using Hipstr.Core.Models;
using System.Collections.ObjectModel;

namespace Hipstr.Client.Views
{
	public interface IFilterable
	{
		ObservableCollection<FilterItem> Filters { get; }
	}
}