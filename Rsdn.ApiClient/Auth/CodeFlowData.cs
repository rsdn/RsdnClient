namespace Rsdn.ApiClient.Auth
{
	public class CodeFlowData
	{
		public string AuthUri { get; }
		public string State { get; }
		public string CodeVerifier { get; }
		public string RedirectUri { get; }

		public CodeFlowData(string authUri, string state, string codeVerifier, string redirectUri)
		{
			AuthUri = authUri;
			State = state;
			CodeVerifier = codeVerifier;
			RedirectUri = redirectUri;
		}
	}
}