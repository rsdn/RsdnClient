using System;

namespace Rsdn.ApiClient.Tests
{
	public class TestsBase
	{
		public const string TestClientID = "dev_test_client";
		public const string TestClientSecret = "O86i6njN4Juxrr71HItllA";
		public const string TestUserLogin = "RsdnDevUser";
		public const string TestUserPassword = "jetlag@300";
		public const int TestForumID = 43;
		public static readonly Uri ServiceUri = new Uri("https://localhost:44389");

		protected readonly RsdnApiClient Client =
			RsdnClientHelpers.CreateClient(
				ServiceUri,
				TestClientID,
				TestClientSecret,
				TestUserLogin,
				TestUserPassword,
				"offline_access");

		protected readonly RsdnApiClient AnonymousClient = RsdnClientHelpers.CreateAnonymousClient(ServiceUri);
	}
}