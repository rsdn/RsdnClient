using System.Linq;
using System.Threading.Tasks;
using CodeJam.Strings;
using NUnit.Framework;
using Rsdn.Api.Models.Messages;

namespace Rsdn.ApiClient.Tests
{
	[TestFixture]
	public class MessagesTests : TestsBase
	{
		[Test]
		public async Task GetMessage()
		{
			var msg = await Client.Messages.GetMessageAsync(7484481, true, true, true);
			Assert.NotNull(msg);
			Assert.NotNull(msg.Rates);
			Assert.NotNull(msg.Body);
			Assert.NotNull(msg.Body.Text);
		}

		[Test]
		public async Task GetMessages()
		{
			var msgs = await Client.Messages.GetMessagesAsync(
				limit: 10,
				offset: 10,
				withBodies: true);
			Assert.NotNull(msgs);
			Assert.AreEqual(10, msgs.Offset);
			Assert.NotZero(msgs.Total);
			Assert.NotNull(msgs.Items);
			Assert.IsNotEmpty(msgs.Items);
			Assert.IsTrue(msgs.Items.All(m => m.Body != null && m.Body.Text.NotNullNorEmpty()));
		}

		[Test]
		public async Task AddMessage()
		{
			const string subj = "O La La";
			const string body = "He says: [b]O La La[/b]!";
			const string tags = "Test OLaLa";
			var msg = await Client.Messages.AddMessageAsync(
				new AddMessageRequest
				{
					Body = body,
					ForumID = TestForumID,
					Subject = subj,
					Tags = tags
				});
			Assert.NotNull(msg);
			var me = await Client.Accounts.GetMeAsync();
			Assert.AreEqual(me.ID, msg.Author.ID);
			Assert.AreEqual(TestForumID, msg.ForumID);
			Assert.AreEqual(tags, msg.Tags);

			msg = await Client.Messages.GetMessageAsync(msg.ID, true, true);
			Assert.NotNull(msg);
			Assert.AreEqual(me.ID, msg.Author.ID);
			Assert.AreEqual(body, msg.Body?.Text);
			Assert.AreEqual(TestForumID, msg.ForumID);
			Assert.AreEqual(tags, msg.Tags);
			Assert.IsEmpty(msg.Rates?.Rates);
		}
	}
}