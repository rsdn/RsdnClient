using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using CodeJam;
using CodeJam.Threading;
using JetBrains.Annotations;
using Rsdn.Api.Models.Auth;
using Rsdn.ApiClient.Auth;
using Rsdn.ApiClient.Retry;

namespace Rsdn.ApiClient
{
	[PublicAPI]
	public static class RsdnClientHelpers
	{
		public static TokenFactory GetAccessTokenFactory(
			[NotNull] this RsdnApiAuthenticator authenticator,
			[NotNull] string login,
			[NotNull] string password)
		{
			Code.NotNull(authenticator, nameof(authenticator));

			AuthTokenResponse token = null;
			var tokenLock = new AsyncLock();
			return async ct =>
			{
				if (token != null && token.ExpiresOn >= DateTimeOffset.UtcNow)
					return token.AccessToken;
				using (await tokenLock.AcquireAsync(ct))
					if (token == null || token.ExpiresOn < DateTimeOffset.UtcNow)
						token = token == null
							? await authenticator.GetTokenByPasswordAsync(login, password, ct)
							: await authenticator.RefreshTokenAsync(token.RefreshToken, ct)
							  ?? await authenticator.GetTokenByPasswordAsync(login, password, ct);
				return token.AccessToken;
			};
		}

		public static TokenFactory GetAccessTokenFactory(
			[NotNull] this RsdnApiAuthenticator authenticator,
			[NotNull] Func<AuthTokenResponse> getToken,
			[NotNull] Action<AuthTokenResponse> setToken)
		{
			Code.NotNull(authenticator, nameof(authenticator));

			var tokenLock = new AsyncLock();
			return async ct =>
			{
				var token = getToken();
				if (token == null)
					return null;
				if (token.ExpiresOn >= DateTimeOffset.UtcNow)
					return token.AccessToken;
				using (await tokenLock.AcquireAsync(ct))
					if (token.ExpiresOn < DateTimeOffset.UtcNow)
					{
						token = await authenticator.RefreshTokenAsync(token.RefreshToken, ct);
						setToken(token);
					}
				return token?.AccessToken;
			};
		}

		public static TokenFactory GetAccessTokenFactory(
			[NotNull] this RsdnApiAuthenticator authenticator,
			[NotNull] CodeFlowData flowData,
			[NotNull] IDictionary<string, string> redirectParams)
		{
			Code.NotNull(authenticator, nameof(authenticator));
			Code.NotNull(flowData, nameof(flowData));
			Code.NotNull(redirectParams, nameof(redirectParams));

			AuthTokenResponse token = null;
			var tokenLock = new AsyncLock();
			return async ct =>
			{
				if (token == null)
					using (await tokenLock.AcquireAsync(ct))
						if (token == null)
						{
							token = await authenticator.GetTokenByCodeAsync(flowData, redirectParams, ct);
							return token.AccessToken;
						}
				if (token.ExpiresOn >= DateTimeOffset.UtcNow)
					return token.AccessToken;
				using (await tokenLock.AcquireAsync())
					if (token.ExpiresOn < DateTimeOffset.UtcNow)
						token = await authenticator.RefreshTokenAsync(token.RefreshToken, ct);
				return token?.AccessToken;
			};
		}

		private static HttpClient CreateHttpClient(Uri serviceUri, bool useRetries)
		{
			HttpMessageHandler handler = new HttpClientHandler
			{
				Proxy = null // SEE https://stackoverflow.com/questions/2519655/httpwebrequest-is-extremely-slow/3603413#3603413
			}; // Default message handler

			if (useRetries)
			{
				var retryPolicy = new RsdnClientRetryPolicy();
				handler = new RetryPolicyHandler(retryPolicy, handler);
			}

			return new HttpClient(handler)
			{
				BaseAddress = serviceUri,
				DefaultRequestHeaders =
				{
					UserAgent = { ClientInfo.DefaultUserAgentHeader }
				}
			};
		}

		public static RsdnApiAuthenticator CreateAuthenticator(
			Uri serviceUri,
			string clientId,
			string clientSecret,
			string scope,
			bool useRetries = true) =>
			new RsdnApiAuthenticator(
				CreateHttpClient(serviceUri, useRetries),
				clientId,
				clientSecret,
				scope);

		public static RsdnApiClient CreateClient(Uri serviceUri, TokenFactory tokenFactory, bool useRetires = true) =>
			new RsdnApiClient(CreateHttpClient(serviceUri, useRetires), tokenFactory);

		public static RsdnApiClient CreateClient(
			Uri serviceUri,
			string clientId,
			string clientSecret,
			string login,
			string password,
			string scope,
			bool useRetires = true) =>
			new RsdnApiClient(
				CreateHttpClient(serviceUri, useRetires),
				CreateAuthenticator(serviceUri, clientId, clientSecret, scope, useRetires)
					.GetAccessTokenFactory(login, password));

		public static RsdnApiClient CreateClient(
			Uri serviceUri,
			string clientId,
			string clientSecret,
			[NotNull] Func<AuthTokenResponse> getToken,
			[NotNull] Action<AuthTokenResponse> setToken,
			string scope,
			bool useRetires = true) =>
			new RsdnApiClient(
				CreateHttpClient(serviceUri, useRetires),
				CreateAuthenticator(serviceUri, clientId, clientSecret, scope, useRetires)
					.GetAccessTokenFactory(getToken, setToken));

		public static RsdnApiClient CreateAnonymousClient(
			Uri serviceUri,
			bool useRetires = true) =>
			new RsdnApiClient(
				CreateHttpClient(serviceUri, useRetires),
				ct => Task.FromResult<string>(null));
	}
}