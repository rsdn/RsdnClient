using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rsdn.Api.Models.Rates;

namespace Rsdn.ApiClient
{
	[PublicAPI]
	public class RatesApi : ApiBase
	{
		public RatesApi(HttpClient httpClient, TokenFactory tokenFactory, JsonSerializerOptions serializerOptions)
			: base(httpClient, tokenFactory, serializerOptions)
		{}

		public Task<RateInfo> GetRate(int rateID, CancellationToken cancellation = default) =>
			GetAsync<RateInfo>($"rates/{rateID}", cancellation, true);

		public async Task<RateInfo> AddRate(RateType type, int messageID, CancellationToken cancellation = default) =>
			(await PostAsync<AddRateRequest, RateInfo>(
				"rates",
				new AddRateRequest
				{
					MessageID = messageID,
					Type = type
				},
				cancellation))
			.Result;

		public Task DeleteRatesAsync(int messageID, CancellationToken cancellation = default) =>
			DeleteAsync($"rates?messageID={messageID}", cancellation);
	}
}