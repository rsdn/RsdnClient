using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Rsdn.Api.Models;
using Rsdn.Api.Models.ReadMarks;

namespace Rsdn.ApiClient
{
	public class ReadMarksApi : ApiBase
	{
		private const string _baseUri = "readMarks";

		public ReadMarksApi(HttpClient httpClient, TokenFactory tokenFactory, JsonSerializerOptions serializerOptions)
			: base(httpClient, tokenFactory, serializerOptions)
		{}

		public Task<PagedResult<ReadMarkInfo>> GetReadMarksAsync(
				int? limit = default,
				int? offset = default,
				int? forumID = default,
				int? topicID = default,
				int? messageID = default,
				CancellationToken cancellation = default) =>
			GetAsync<PagedResult<ReadMarkInfo>>(
				QueryHelpers.AddQueryString(
					_baseUri,
					new Dictionary<string, string>()
						.AddToQuery(nameof(limit), limit)
						.AddToQuery(nameof(offset), offset)
						.AddToQuery(nameof(forumID), forumID)
						.AddToQuery(nameof(topicID), topicID)
						.AddToQuery(nameof(messageID), messageID)),
				cancellation);

		public async Task AddReadMarksAsync(int[] messageIDs, CancellationToken cancellation = default) =>
			await PostAsync<int[], EmptyResponse>($"{_baseUri}/bulk", messageIDs, cancellation);

		public Task DeleteReadMarksAsync(
				int? forumID = default,
				int? topicID = default,
				int? messageID = default,
				CancellationToken cancellation = default) =>
			DeleteAsync(
				QueryHelpers.AddQueryString(
					_baseUri,
					new Dictionary<string, string>()
						.AddToQuery(nameof(forumID), forumID)
						.AddToQuery(nameof(topicID), topicID)
						.AddToQuery(nameof(messageID), messageID)),
				cancellation);
	}
}