using System.Net;
using System.Threading.Tasks;
using CodeJam.Strings;
using NUnit.Framework;
using Rsdn.Api.Models.Auth;
using Rsdn.Api.Models.Errors;
using Rsdn.ApiClient.Auth;

namespace Rsdn.ApiClient.Tests
{
	public class AuthTests
	{
		private RsdnApiAuthenticator _authenticator;

		[SetUp]
		public void Setup()
		{
			_authenticator =
				RsdnClientHelpers.CreateAuthenticator(
					TestsBase.ServiceUri,
					TestsBase.TestClientID,
					TestsBase.TestClientSecret,
					"offline_access",
					false);
		}

		[Test]
		public async Task TokenByPassword()
		{
			var token = await _authenticator.GetTokenByPasswordAsync(
				TestsBase.TestUserLogin,
				TestsBase.TestUserPassword);
			Assert.IsTrue(token.AccessToken.NotNullNorWhiteSpace());
			Assert.IsTrue(token.RefreshToken.NotNullNorWhiteSpace());
			Assert.NotZero(token.ExpiresIn);
		}

		[Test]
		public void BadPassword()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				() => _authenticator.GetTokenByPasswordAsync("Bob", "P@sw0rd"));
			Assert.AreEqual($"{WellKnownAuthErrors.BaseUri}/InvalidGrant", ex.ErrorCode);
		}

		[Test]
		public async Task RefreshToken()
		{
			var token = await _authenticator.GetTokenByPasswordAsync(
				TestsBase.TestUserLogin,
				TestsBase.TestUserPassword);
			var newToken = await _authenticator.RefreshTokenAsync(token.RefreshToken);
			Assert.IsNotEmpty(newToken.AccessToken);
			Assert.NotZero(newToken.ExpiresIn);
			Assert.AreNotEqual(token.AccessToken, newToken.AccessToken);
		}

		[Test]
		public void BadClientID()
		{
			var authenticator =
				RsdnClientHelpers.CreateAuthenticator(
					TestsBase.ServiceUri,
					"BadID",
					TestsBase.TestClientSecret,
					"offline_access",
					false);
			var ex = Assert.ThrowsAsync<RsdnServiceException>(async () =>
				await authenticator.GetTokenByPasswordAsync(
					TestsBase.TestUserLogin,
					TestsBase.TestUserPassword));
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
			Assert.AreEqual($"{WellKnownAuthErrors.BaseUri}/InvalidClient", ex.ErrorCode);
		}

		[Test]
		public void BadClientSecret()
		{
			var authenticator =
				RsdnClientHelpers.CreateAuthenticator(
					TestsBase.ServiceUri,
					TestsBase.TestClientID,
					"BadSecret",
					"offline_access",
					false);
			var ex = Assert.ThrowsAsync<RsdnServiceException>(async () =>
				await authenticator.GetTokenByPasswordAsync(
					TestsBase.TestUserLogin,
					TestsBase.TestUserPassword));
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
			Assert.AreEqual($"{WellKnownAuthErrors.BaseUri}/InvalidClient", ex.ErrorCode);
		}

		[Test]
		public async Task NonConfidentialClient()
		{
			var authenticator = RsdnClientHelpers.CreateAuthenticator(
				TestsBase.ServiceUri,
				"test_public_client",
				null,
				"offline_access",
				false);
			var token = await authenticator.GetTokenByPasswordAsync(
				TestsBase.TestUserLogin,
				TestsBase.TestUserPassword);
			Assert.IsTrue(token.AccessToken.NotNullNorWhiteSpace());
			Assert.IsTrue(token.RefreshToken.NotNullNorWhiteSpace());
			Assert.NotZero(token.ExpiresIn);
		}
	}
}