#if DEBUG
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Rsdn.ApiClient
{
	public class DebugApi : ApiBase
	{
		public DebugApi(HttpClient httpClient, TokenFactory tokenFactory, JsonSerializerOptions serializerOptions)
			: base(httpClient, tokenFactory, serializerOptions)
		{}

		public Task ErrorAsync(HttpStatusCode status, CancellationToken cancellation = default) =>
			GetAsync<object>("debug/error?status=" + status, cancellation);
	}
}
#endif