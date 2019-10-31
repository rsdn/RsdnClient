using System;
using JetBrains.Annotations;
using Rsdn.Api.Models.Accounts;

namespace Rsdn.Api.Models.Messages
{
	public class MessageInfo
	{
		public int ID { get; set; }
		public int ForumID { get; set; }
		public int TopicID { get; set; }
		public ShortPublicAccountInfo Author { get; set; }
		public string Subject { get; set; }
		public DateTimeOffset CreatedOn { get; set; }
		public DateTimeOffset? UpdatedOn { get; set; }
		public bool IsTopic { get; set; }
		public int ParentID { get; set; }
		public string Tags { get; set; }
		public short? AnswersCount { get; set; }
		public bool? IsRead { get; set; }
		public int? TopicUnreadCount { get; set; }

		[CanBeNull]
		public MessageRates Rates { get; set; }

		[CanBeNull]
		public MessageBody Body { get; set; }
	}
}