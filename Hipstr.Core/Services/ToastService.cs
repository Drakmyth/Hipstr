using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;
using JetBrains.Annotations;

namespace Hipstr.Core.Services
{
	[UsedImplicitly]
	public class ToastService : IToastService
	{
		public void ShowCommunicationErrorToast(string message)
		{
			var content = new ToastContent
			{
				Visual = new ToastVisual
				{
					BindingGeneric = new ToastBindingGeneric
					{
						Children =
						{
							new AdaptiveText
							{
								Text = "Communication Error"
							},
							new AdaptiveText
							{
								Text = message
							}
						}
					}
				}
			};

			var notification = new ToastNotification(content.GetXml());
			ToastNotificationManager.CreateToastNotifier().Show(notification);
		}

		public void ShowUnknownErrorToast(string message)
		{
			var content = new ToastContent
			{
				Visual = new ToastVisual
				{
					BindingGeneric = new ToastBindingGeneric
					{
						Children =
						{
							new AdaptiveText
							{
								Text = "Unknown Error"
							},
							new AdaptiveText
							{
								Text = message
							}
						}
					}
				}
			};

			var notification = new ToastNotification(content.GetXml());
			ToastNotificationManager.CreateToastNotifier().Show(notification);
		}
	}
}