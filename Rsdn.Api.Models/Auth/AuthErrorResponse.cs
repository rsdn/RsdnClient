using System;

namespace Rsdn.Api.Models.Auth
{
	public class AuthErrorResponse
	{
		public AuthErrorType Error { get; set; }
		public string ErrorDescription { get; set; }
		public Uri ErrorUri { get; set; }
	}
}