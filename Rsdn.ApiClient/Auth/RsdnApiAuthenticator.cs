using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CodeJam.Collections;
using CodeJam.Strings;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Rsdn.Api.Models.Auth;
using Rsdn.Api.Models.Errors;

namespace Rsdn.ApiClient.Auth
{
	/// <summary>
	/// OAuth2 authentication implementation for RSDN API.
	/// </summary>
	[PublicAPI]
	public class RsdnApiAuthenticator
	{
		private readonly string _clientId;
		[CanBeNull]
		private readonly string _clientSecret;
		private readonly string _scope;
		private readonly HttpClient _httpClient;

		public RsdnApiAuthenticator(
			HttpClient httpClient,
			string clientId,
			[CanBeNull] string clientSecret,
			string scope)
		{
			_clientId = clientId;
			_clientSecret = clientSecret;
			_scope = scope;

			_httpClient = httpClient;
		}

		public async Task<AuthTokenResponse> GetTokenByPasswordAsync(
			string userName,
			string password,
			CancellationToken cancellation = default)
		{
			var response = await _httpClient.PostAsync(
				"connect/token",
				new StringContent(
					"grant_type=password" +
					$"&username={Uri.EscapeDataString(userName)}" +
					$"&password={Uri.EscapeDataString(password)}" +
					$"&client_id={_clientId}" +
					(_clientSecret == null ? "" : $"&client_secret={_clientSecret}") +
					$"&scope={_scope}",
					Encoding.UTF8,
					"application/x-www-form-urlencoded"),
				cancellation);
			return await GetTokenResponseAsync(response);
		}

		[ItemCanBeNull]
		public async Task<AuthTokenResponse> RefreshTokenAsync(string refreshToken, CancellationToken cancellation = default)
		{
			if (refreshToken.IsNullOrEmpty())
				return null;
			var response = await _httpClient.PostAsync(
				"connect/token",
				new StringContent(
					"grant_type=refresh_token" +
					$"&refresh_token={refreshToken}" +
					$"&client_id={_clientId}" +
					(_clientSecret == null ? "" : $"&client_secret={_clientSecret}") +
					$"&scope={_scope}",
					Encoding.UTF8,
					"application/x-www-form-urlencoded"),
				cancellation);
			if (response.StatusCode == HttpStatusCode.BadRequest
			) // something wrong with arguments (e.g. invalid or expired refresh token)
				return null;
			return await GetTokenResponseAsync(response);
		}

		public CodeFlowData GetCodeFlowData(string redirectUri)
		{
			// Generates state and PKCE values.
			var state = RandomBase64Url(32);
			var codeVerifier = RandomBase64Url(32);
			var codeChallenge = Base64UrlTextEncoder.Encode(Sha256(codeVerifier));

			var uri = QueryHelpers.AddQueryString(
				new Uri(_httpClient.BaseAddress, "connect/auth").ToString(),
				new Dictionary<string, string>
				{
					{"response_type", "code"},
					{"redirect_uri", redirectUri},
					{"client_id", _clientId},
					{"scope", _scope},
					{"state", state},
					{"code_challenge", codeChallenge},
					{"code_challenge_method", "S256"}
				});

			return new CodeFlowData(uri, state, codeVerifier, redirectUri);
		}

		public async Task<AuthTokenResponse> GetTokenByCodeAsync(
			CodeFlowData flowData,
			IDictionary<string, string> redirectParams,
			CancellationToken cancellation = default)
		{
			if (redirectParams.TryGetValue("error", out var error))
				throw new Exception($"Authentication error: {error}");
			if (!redirectParams.TryGetValue("code", out var code)
			    || !redirectParams.TryGetValue("state", out var state)
			    || !StringComparer.Ordinal.Equals(state, flowData.State))
				throw new Exception("Malformed server response.");
			var body =
				$"code={code}" +
				$"&redirect_uri={Uri.EscapeDataString(flowData.RedirectUri)}" +
				$"&client_id={_clientId}" +
				$"&code_verifier={flowData.CodeVerifier}" +
				"&scope=&grant_type=authorization_code";
			var request = new HttpRequestMessage(HttpMethod.Post, "/connect/token")
			{
				Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded"),
				Headers = { Accept = { new MediaTypeWithQualityHeaderValue("application/json")}}
			};
			return await GetTokenResponseAsync(await _httpClient.SendAsync(request, cancellation));
		}

		/// <summary>
		///   Returns URI-safe data with a given input length.
		/// </summary>
		/// <param name="length">Input length (nb. output will be longer)</param>
		private static string RandomBase64Url(uint length)
		{
			var rng = new RNGCryptoServiceProvider();
			var bytes = new byte[length];
			rng.GetBytes(bytes);
			return Base64UrlTextEncoder.Encode(bytes);
		}

		/// <summary>
		///   Returns the SHA256 hash of the input string.
		/// </summary>
		private static byte[] Sha256(string input)
		{
			var bytes = Encoding.ASCII.GetBytes(input);
			var sha256 = new SHA256Managed();
			return sha256.ComputeHash(bytes);
		}

		private static async Task<AuthTokenResponse> GetTokenResponseAsync(HttpResponseMessage response)
		{
			var content = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode)
			{
				var token = JsonSerializer.Deserialize<AuthTokenResponse>(content);
				token.ExpiresOn = DateTimeOffset.UtcNow.AddSeconds(token.ExpiresIn - 60); // 1 min for server response delay
				return token;
			}

			// OAuth2 error
			if (response.StatusCode == HttpStatusCode.BadRequest || response.StatusCode == HttpStatusCode.Unauthorized)
			{
				var err = GetErrorResponse(content);
				throw new RsdnServiceException(
					new ErrorType(response.StatusCode, WellKnownAuthErrors.BaseUri + err.Error),
					ErrorSeverity.UserError,
					$"Authentication error: {err.Error}." +
					$"{(err.ErrorDescription.NotNullNorWhiteSpace() ? $" {err.ErrorDescription}." : "")}" +
					$"{(err.ErrorUri != null ? $" See {err.ErrorUri}." : "")}");
			}

			// unknown error
			throw new RsdnServiceException(
				WellKnownErrors.InternalServerError,
				ErrorSeverity.Critical,
				$"Unknown error: {response.StatusCode}.{(content.NotNullNorWhiteSpace() ? $" {content}." : "")}");
		}

		private static AuthErrorResponse GetErrorResponse(string content)
		{
			// Deserialize error manually, because string enums yet not supported by serializer.
			var values = JsonSerializer.Deserialize<Dictionary<string, string>>(content);

			var result = new AuthErrorResponse
			{
				ErrorDescription = values.TryGetValue("error_description", out var desc) ? desc : "",
				ErrorUri = values.GetValueOrDefault(
					"error_uri",
					(k, v) => Uri.TryCreate(v, UriKind.RelativeOrAbsolute, out var uri)
						? uri
						: null)
			};
			switch (values["error"])
			{
				case "invalid_request" :
					result.Error = AuthErrorType.InvalidRequest;
					break;
				case "invalid_client" :
					result.Error = AuthErrorType.InvalidClient;
					break;
				case "invalid_grant" :
					result.Error = AuthErrorType.InvalidGrant;
					break;
				case "invalid_scope" :
					result.Error = AuthErrorType.InvalidScope;
					break;
				case "unauthorized_client" :
					result.Error = AuthErrorType.UnauthorizedClient;
					break;
				case "unsupported_grant_type" :
					result.Error = AuthErrorType.UnsupportedGrantType;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			return result;
		}
	}
}