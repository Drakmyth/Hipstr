using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Hipstr.Core.Services
{
	[UsedImplicitly]
	public class HipstrHttpClient : IHttpClient
	{
		private readonly HttpClient _httpClient;

		public HttpRequestHeaders DefaultRequestHeaders => _httpClient.DefaultRequestHeaders;

		public HipstrHttpClient()
		{
			_httpClient = new HttpClient();
		}

		public async Task<HttpResponseMessage> GetAsync(Uri requestUri)
		{
			try
			{
				HttpResponseMessage response = await _httpClient.GetAsync(requestUri);
				if (!response.IsSuccessStatusCode)
				{
					// HipChat returned a non-200 range status code. We shouldn't have given the user an action that
					// wasn't valid, so we report the error.

					// TODO: Add automatic error reporting
					/*
					 * string errorMessage = "Received an error response from the HipChat server.";
					 * 
					 * if (telemetryEnabled) {
					 *	   sendErrorReport(response);
					 *     errorMessage += " An error report has been sent to the developer.";
					 * }
					 * 
					 * errorMessage = " Please go back, refresh the page, and try again.";
					 */

					throw new CommunicationException($"Received an error response from the HipChat server. Code: {response.StatusCode} - Reason: {response.Content}");
				}

				if (!response.RequestMessage.RequestUri.Equals(requestUri))
				{
					// Our request got redirected. This means we probably hit WebSense or something similar, so let's fail gracefully.
					throw new CommunicationException($"The request to the HipChat server was intercepted, possibly by a Web Filter. Please allow {requestUri.DnsSafeHost} as an exception in the filter's whitelist.");
				}

				return response;
			}
			catch (HttpRequestException e)
			{
				// We failed to connect to the HipChat server. This most likely means the user has no internet connection.
				throw new CommunicationException("Communication with the HipChat server has failed. Please check your internet connection and try again.", e);
			}
		}
	}
}