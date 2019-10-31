using System;
using Rsdn.Api.Models.Accounts;

namespace Rsdn.Api.Models.Rates
{
	public class RateInfo
	{
		public int ID { get; set; }
		public RateType Type { get; set; }
		public ShortPublicAccountInfo RatedBy { get; set; }
		public DateTimeOffset RatedOn { get; set; }
		public int? RateBase { get; set; }
		public int? RateValue { get; set; }
	}
}