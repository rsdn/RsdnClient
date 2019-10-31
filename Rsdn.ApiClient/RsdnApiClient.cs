using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rsdn.Api.Models;

namespace Rsdn.ApiClient
{
	public delegate Task<string> TokenFactory(CancellationToken cancellation = default);
	/// <summary>
	/// RSDN API client.
	/// </summary>
	[PublicAPI]
	public class RsdnApiClient
	{
		public RsdnApiClient(
			HttpClient httpClient,
			TokenFactory tokenFactory)
		{
			#if DEBUG
			Debug = new DebugApi(httpClient, tokenFactory, SerializerSettings.Default);
			#endif
			Accounts = new AccountsApi(httpClient, tokenFactory, SerializerSettings.Default);
			Forums = new ForumsApi(httpClient, tokenFactory, SerializerSettings.Default);
			Messages = new MessagesApi(httpClient, tokenFactory, SerializerSettings.Default);
			Rates = new RatesApi(httpClient, tokenFactory, SerializerSettings.Default);
			ReadMarks = new ReadMarksApi(httpClient, tokenFactory, SerializerSettings.Default);
		}

		#if DEBUG
		public DebugApi Debug { get; }
		#endif
		public AccountsApi Accounts { get; }
		public ForumsApi Forums { get; }
		public MessagesApi Messages { get; }
		public RatesApi Rates { get; }
		public ReadMarksApi ReadMarks { get; }
	}
}