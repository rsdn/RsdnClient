using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Rsdn.Api.Models.Accounts;

namespace Rsdn.ApiClient
{
	public class AccountsApi : ApiBase
	{
		public AccountsApi(HttpClient httpClient, TokenFactory tokenFactory, JsonSerializerOptions serializerOptions) : 
			base(httpClient, tokenFactory, serializerOptions)
		{}

		public Task<AccountInfo> GetMeAsync(CancellationToken cancellation = default) =>
			GetAsync<AccountInfo>("accounts/me", cancellation);
	}
}