using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rsdn.Api.Models.Forums;

namespace Rsdn.ApiClient
{
	public class ForumsApi : ApiBase
	{
		public ForumsApi(
			HttpClient httpClient,
			TokenFactory tokenFactory,
			JsonSerializerOptions serializerOptions)
			: base(httpClient, tokenFactory, serializerOptions)
		{}

		[ItemNotNull]
		public async Task<ForumDescription[]> GetForumsAsync(CancellationToken cancellation = default) =>
			await GetAsync<ForumDescription[]>("forums", cancellation)
				?? Array.Empty<ForumDescription>();

		[ItemCanBeNull]
		public async Task<ForumInfo> GetForumAsync(int forumID, CancellationToken cancellation = default) =>
			await GetAsync<ForumInfo>($"forums/{forumID}", cancellation, true);
	}
}