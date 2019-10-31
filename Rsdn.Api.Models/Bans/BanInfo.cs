using System;

namespace Rsdn.Api.Models.Bans
{
	public class BanInfo
	{
		public string[] Reasons { get; set; }
		public DateTimeOffset BannedTill { get; set; }
	}
}