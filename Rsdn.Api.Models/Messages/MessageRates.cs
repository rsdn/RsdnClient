namespace Rsdn.Api.Models.Messages
{
	public class MessageRates
	{
		public MessageRate[] Rates { get; set; }
		public int SmileCount { get; set; }
		public int LikeCount { get; set; }
		public int DislikeCount { get; set; }
		public int TotalRate { get; set; }
	}
}