namespace Rsdn.Api.Models
{
	public class PagedResult<TItem>
	{
		public TItem[] Items { get; set; }
		public int Total { get; set; }
		public int Offset { get; set; }
	}
}