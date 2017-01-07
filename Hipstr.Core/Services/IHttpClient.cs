using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHttpClient
	{
		HttpRequestHeaders DefaultRequestHeaders { get; }
		Task<HttpResponseMessage> PostAsync<T>(Uri requestUri, T payload);
		Task<HttpResponseMessage> GetAsync(Uri requestUri);
	}
}