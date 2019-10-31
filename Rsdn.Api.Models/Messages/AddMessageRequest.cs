namespace Rsdn.Api.Models.Messages
{
	public class AddMessageRequest
	{
		public int ForumID { get; set; }
		public int? ReplyToMessageID { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public string Tags { get; set; }
		public bool SubscribeToAnswers { get; set; }
	}
}