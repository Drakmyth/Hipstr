﻿using Microsoft.Toolkit.Uwp.Notifications;
using Windows.UI.Notifications;

namespace Hipstr.Core.Services
{
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

			ToastNotification notification = new ToastNotification(content.GetXml());
			ToastNotificationManager.CreateToastNotifier().Show(notification);
		}
	}
}