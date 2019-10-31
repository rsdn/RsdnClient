namespace Rsdn.Api.Models.Auth
{
	public enum AuthErrorType
	{
		InvalidRequest,
		InvalidClient,
		InvalidGrant,
		InvalidScope,
		UnauthorizedClient,
		UnsupportedGrantType
	}
}