namespace Hipstr.Core.Services
{
	public interface IToastService
	{
		void ShowCommunicationErrorToast(string message);
		void ShowUnknownErrorToast(string message);
	}
}