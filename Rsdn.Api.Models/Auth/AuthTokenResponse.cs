using System;
using System.Text.Json.Serialization;
using JetBrains.Annotations;

namespace Rsdn.Api.Models.Auth
{
	[PublicAPI]
	public class AuthTokenResponse
	{
		[JsonPropertyName("access_token")]
		public string AccessToken { get; set; }

		[JsonPropertyName("expires_in")]
		public int ExpiresIn { get; set; }

		[JsonPropertyName("refresh_token")]
		public string RefreshToken { get; set; }

		[JsonPropertyName("scope")]
		public string Scope { get; set; }

		public DateTimeOffset ExpiresOn { get; set; }
	}
}