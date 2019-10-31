namespace Rsdn.Api.Models.Accounts
{
	public class ShortPublicAccountInfo
	{
		public int ID { get; set; }
		public string DisplayName { get; set; }
		public string GravatarHash { get; set; }
		public AccountRole Role { get; set; }
	}
}