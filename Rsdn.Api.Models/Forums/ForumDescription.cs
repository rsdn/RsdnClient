namespace Rsdn.Api.Models.Forums
{
	public class ForumDescription
	{
		public int ID { get; set; }
		public string Code { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public ForumGroupInfo ForumGroup { get; set; }
		public bool IsInTop { get; set; }
		public bool IsSiteSubject { get; set; }
		public bool IsService { get; set; }
		public bool IsRated { get; set; }
		public short RateLimit { get; set; }
		public bool IsWriteAllowed { get; set; }
	}
}