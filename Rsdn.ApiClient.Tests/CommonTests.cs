using System.Net;
using NUnit.Framework;
using Rsdn.Api.Models.Errors;

namespace Rsdn.ApiClient.Tests
{
	[TestFixture]
	public class CommonTests : TestsBase
	{
		[Test]
		public void Error()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				async () => await Client.Debug.ErrorAsync(HttpStatusCode.NotFound));
			Assert.AreEqual(HttpStatusCode.NotFound, ex.StatusCode);
			Assert.AreEqual($"{WellKnownErrors.BaseUri}/debug/error", ex.ErrorCode);
		}
	}
}