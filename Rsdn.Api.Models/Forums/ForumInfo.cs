namespace Rsdn.Api.Models.Forums
{
	public class ForumInfo : ForumDescription
	{
		public int TotalMessages { get; set; }
		public int TodayMessages { get; set; }
	}
}