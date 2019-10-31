using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using CodeJam;

namespace Rsdn.ApiClient.Retry
{
	internal class RetryPolicyHandler : DelegatingHandler
	{
		private readonly IRetryPolicy _retryPolicy;

		/// <summary>
		/// Initialize instance.
		/// </summary>
		public RetryPolicyHandler(
			IRetryPolicy retryPolicy, 
			HttpMessageHandler innerHandler):
			base(innerHandler)
		{
			Code.NotNull(retryPolicy, nameof(retryPolicy));
			_retryPolicy = retryPolicy;
		}

		/// <inheritdoc />
		protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellation)
		{
			return await _retryPolicy.ExecuteAsync(async ct => await base.SendAsync(request, ct), cancellation);
		}
	}
}