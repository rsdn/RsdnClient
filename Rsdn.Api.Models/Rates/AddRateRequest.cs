namespace Rsdn.Api.Models.Rates
{
	public class AddRateRequest
	{
		public RateType Type { get; set; }
		public int MessageID { get; set; }
	}
}