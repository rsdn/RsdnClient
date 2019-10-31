using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using Rsdn.Api.Models.Errors;
using Rsdn.Api.Models.Messages;

namespace Rsdn.ApiClient.Tests
{
	[TestFixture]
	public class ReadMarksTests : TestsBase
	{
		private int _msgID;

		[SetUp]
		public async Task SetUp()
		{
			var msg = await Client.Messages.AddMessageAsync(
				new AddMessageRequest
				{
					ForumID = TestForumID,
					Subject = "RateMarksTest",
					Body = "Rate marks test.",
					Tags = "test"
				});
			_msgID = msg.ID;
		}

		[Test]
		public async Task AddDelete()
		{
			var msg = await Client.Messages.GetMessageAsync(_msgID, withReadMarks: true);
			Assert.IsNotNull(msg);
			Assert.IsFalse(msg.IsRead);

			await Client.ReadMarks.AddReadMarksAsync(new []{_msgID});

			var rms = await Client.ReadMarks.GetReadMarksAsync(messageID: _msgID);
			Assert.IsNotNull(rms?.Items);
			Assert.AreEqual(1, rms.Items.Length);
			Assert.AreEqual(_msgID, rms.Items[0].MessageID);
			Assert.AreEqual(TestForumID, rms.Items[0].ForumID);

			msg = await Client.Messages.GetMessageAsync(_msgID, withReadMarks: true);
			Assert.IsTrue(msg?.IsRead);

			await Client.ReadMarks.DeleteReadMarksAsync(messageID: _msgID);

			msg = await Client.Messages.GetMessageAsync(_msgID, withReadMarks: true);
			Assert.IsNotNull(msg);
			Assert.IsFalse(msg.IsRead);

			var reply = await Client.Messages.AddMessageAsync(
				new AddMessageRequest
				{
					ForumID = TestForumID,
					Subject = "RateMarksTest",
					Body = "> Rate marks test.",
					Tags = "test",
					ReplyToMessageID = _msgID
				});

			msg = await Client.Messages.GetMessageAsync(_msgID, withReadMarks: true);
			Assert.AreEqual(1, msg?.TopicUnreadCount);

			await Client.ReadMarks.AddReadMarksAsync(new []{reply.ID});

			msg = await Client.Messages.GetMessageAsync(_msgID, withReadMarks: true);
			Assert.IsNotNull(msg);
			Assert.IsFalse(msg.IsRead);
			Assert.AreEqual(0, msg.TopicUnreadCount);

			reply = await Client.Messages.GetMessageAsync(reply.ID, withReadMarks: true);
			Assert.IsTrue(reply?.IsRead);
		}

		[Test]
		public void AnonAdd()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				() => AnonymousClient.ReadMarks.AddReadMarksAsync(new[] {_msgID}));
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
		}

		[Test]
		public void AnonDelete()
		{
			var ex = Assert.ThrowsAsync<RsdnServiceException>(
				() => AnonymousClient.ReadMarks.DeleteReadMarksAsync(messageID: _msgID));
			Assert.AreEqual(HttpStatusCode.Unauthorized, ex.StatusCode);
		}
	}
}