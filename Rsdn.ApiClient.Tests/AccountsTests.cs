using System.Threading.Tasks;
using NUnit.Framework;

namespace Rsdn.ApiClient.Tests
{
	public class AccountsTests : TestsBase
	{
		[Test]
		public async Task GetMe()
		{
			var me = await Client.Accounts.GetMeAsync();
			Assert.IsNotNull(me);
			Assert.AreEqual(TestUserLogin, me.Login);
			Assert.AreEqual("Rsdn Dev User", me.DisplayName);

			// Second call. Check cache.
			me = await Client.Accounts.GetMeAsync();
			Assert.IsNotNull(me);
			Assert.AreEqual(TestUserLogin, me.Login);
		}
	}
}