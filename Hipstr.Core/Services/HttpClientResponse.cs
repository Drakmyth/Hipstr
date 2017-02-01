using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public class HttpClientResponse<TPayload>
	{
		public HttpStatusCode StatusCode { get; }
		public TPayload Payload { get; }

		private HttpClientResponse(HttpStatusCode statusCode, TPayload payload)
		{
			StatusCode = statusCode;
			Payload = payload;
		}

		public static async Task<HttpClientResponse<TPayload>> FromResponseMessageAsync(HttpResponseMessage response)
		{
			string json = await response.Content.ReadAsStringAsync();
			var payload = JsonConvert.DeserializeObject<TPayload>(json);
			return new HttpClientResponse<TPayload>(response.StatusCode, payload);
		}
	}
}