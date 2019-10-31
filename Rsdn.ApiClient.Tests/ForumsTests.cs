using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Rsdn.ApiClient.Tests
{
	[TestFixture]
	public class ForumsTests : TestsBase
	{
		[Test]
		public async Task GetForums()
		{
			var forums = await Client.Forums.GetForumsAsync();
			Assert.NotNull(forums);
			Assert.IsNotEmpty(forums);
			var allowedToWrite = forums.Count(f => f.IsWriteAllowed);
			Assert.NotZero(allowedToWrite);

			var anonForums = await AnonymousClient.Forums.GetForumsAsync();
			Assert.NotNull(forums);
			Assert.IsNotEmpty(forums);
			Assert.Less(anonForums.Length, forums.Length);
			var anonAllowedToWrite = anonForums.Count(f => f.IsWriteAllowed);
			Assert.Less(anonAllowedToWrite, allowedToWrite);
		}

		[Test]
		public async Task GetForum()
		{
			var forum = await Client.Forums.GetForumAsync(8);
			Assert.NotNull(forum);
			Assert.AreEqual("dotnet", forum.Code);
			Assert.NotZero(forum.TotalMessages);
		}
	}
}