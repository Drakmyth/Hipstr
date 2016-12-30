using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Hipstr.Core.Services
{
	public interface IHttpClient
	{
		HttpRequestHeaders DefaultRequestHeaders { get; }
		Task<HttpResponseMessage> GetAsync(Uri requestUri);
	}
}