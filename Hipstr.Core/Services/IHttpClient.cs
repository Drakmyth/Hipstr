using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHttpClient
	{
		HttpRequestHeaders DefaultRequestHeaders { get; }
		Task<HttpClientResponse<TResponse>> PostAsync<TResponse>(Uri requestUri, object payload);
		Task<HttpClientResponse<TResponse>> GetAsync<TResponse>(Uri requestUri);
	}
}