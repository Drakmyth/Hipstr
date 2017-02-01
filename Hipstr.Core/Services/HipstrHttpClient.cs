using JetBrains.Annotations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.ServiceModel;
using System.Threading.Tasks;

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

		public async Task<HttpClientResponse<TResponse>> GetAsync<TResponse>(Uri requestUri)
		{
			HttpResponseMessage response = await SendRequestAsync(requestUri, async () => await _httpClient.GetAsync(requestUri));
			return await HttpClientResponse<TResponse>.FromResponseMessageAsync(response);
		}

		public async Task<HttpClientResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object payload)
		{
			HttpContent content = new StringContent(JsonConvert.SerializeObject(payload));
			content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
			HttpResponseMessage response = await SendRequestAsync(requestUri, async () => await _httpClient.PostAsync(requestUri, content));
			return await HttpClientResponse<TResponse>.FromResponseMessageAsync(response);
		}

		private static async Task<HttpResponseMessage> SendRequestAsync(Uri requestUri, Func<Task<HttpResponseMessage>> clientRequestMethod)
		{
			try
			{
				HttpResponseMessage response = await clientRequestMethod.Invoke();

				if (!response.RequestMessage.RequestUri.Equals(requestUri))
				{
					// Our request got redirected. This means we probably hit WebSense or something similar, so let's fail gracefully.
					throw new CommunicationException($"The request to the HipChat server was intercepted, possibly by a Web Filter. Please allow {requestUri.DnsSafeHost} as an exception in the filter's whitelist.");
				}

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
					IEnumerable<string> rateLimitHeader;
					if (response.Headers.TryGetValues("X - Ratelimit - Remaining", out rateLimitHeader) && int.Parse(rateLimitHeader.First()) <= 0)
					{
						throw new CommunicationException($"Reached HipChat rate limit. Please wait 5 minutes then try again.");
					}

					throw new CommunicationException($"Received an error response from the HipChat server. Code: {response.StatusCode} - Reason: {response.Content}");
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