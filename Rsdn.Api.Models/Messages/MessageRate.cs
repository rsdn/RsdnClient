using System;
using Rsdn.Api.Models.Accounts;
using Rsdn.Api.Models.Rates;

namespace Rsdn.Api.Models.Messages
{
	public class MessageRate
	{
		/// <summary>
		/// Rate type.
		/// </summary>
		public RateType Type { get; set; }

		/// <summary>
		/// Message rated by.
		/// </summary>
		public ShortPublicAccountInfo RateBy { get; set; }

		/// <summary>
		/// Message rate date.
		/// </summary>
		public DateTimeOffset RatedOn { get; set; }

		/// <summary>
		/// Message rate (x1, x2, x3).
		/// </summary>
		public int? Rate { get; set; }

		/// <summary>
		/// Rate base (calculated from rate author rating).
		/// </summary>
		public int? RateBase { get; set; }
	}
}