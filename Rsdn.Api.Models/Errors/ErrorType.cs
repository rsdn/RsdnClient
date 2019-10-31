using System.Net;

namespace Rsdn.Api.Models.Errors
{
	public class ErrorType
	{
		public ErrorType(HttpStatusCode statusCode, string type)
		{
			StatusCode = statusCode;
			Type = type;
		}

		public HttpStatusCode StatusCode { get; }
		public string Type { get; }
	}
}