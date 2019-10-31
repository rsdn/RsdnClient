using System;
using System.Net;
using System.Net.Http;
using JetBrains.Annotations;
using Rsdn.Api.Models.Errors;

namespace Rsdn.ApiClient.Retry
{
	[PublicAPI]
	public class RsdnClientRetryPolicy : ExponentialBackoffRetryPolicy
	{
		/// <inheritdoc />
		public RsdnClientRetryPolicy(int retryCount, TimeSpan maxBackoff, TimeSpan minBackoff, TimeSpan deltaBackoff) : base(retryCount, maxBackoff, minBackoff, deltaBackoff) { }

		/// <inheritdoc />
		public RsdnClientRetryPolicy(int retryCount) : base(retryCount) { }

		/// <inheritdoc />
		public RsdnClientRetryPolicy() { }

		/// <inheritdoc />
		protected override bool IsTransient(Exception exception)
		{
			switch (exception)
			{
				case HttpRequestException _:
					return true;
				case RsdnServiceException rEx:
					return
						rEx.StatusCode == HttpStatusCode.GatewayTimeout
						|| rEx.StatusCode == HttpStatusCode.RequestTimeout
						|| rEx.StatusCode == HttpStatusCode.ServiceUnavailable;
				default:
					return false;
			}
		}
	}
}