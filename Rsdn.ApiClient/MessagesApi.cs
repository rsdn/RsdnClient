using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.AspNetCore.WebUtilities;
using Rsdn.Api.Models;
using Rsdn.Api.Models.Messages;

namespace Rsdn.ApiClient
{
	public class MessagesApi : ApiBase
	{
		public MessagesApi(
			HttpClient httpClient,
			TokenFactory tokenFactory,
			JsonSerializerOptions serializerOptions)
			: base(httpClient, tokenFactory, serializerOptions)
		{}

		public Task<PagedResult<MessageInfo>> GetMessagesAsync(
			int? limit = default,
			int? offset = default,
			int? forumID = default,
			bool? onlyTopics = default,
			int? topicID = default,
			MessageOrder? order = default,
			bool? withRates = default,
			bool? withBodies = default,
			bool? formatBody = default,
			bool? withReadMarks = default,
			CancellationToken cancellation = default)
		{
			var query = new Dictionary<string, string>(9);
			query
				.AddToQuery(nameof(limit), limit)
				.AddToQuery(nameof(offset), offset)
				.AddToQuery(nameof(forumID), forumID)
				.AddToQuery(nameof(onlyTopics), onlyTopics)
				.AddToQuery(nameof(topicID), topicID)
				.AddToQuery(nameof(order), order)
				.AddToQuery(nameof(withRates), withRates)
				.AddToQuery(nameof(withBodies), withBodies)
				.AddToQuery(nameof(formatBody), formatBody)
				.AddToQuery(nameof(withReadMarks), withReadMarks);
			return GetAsync<PagedResult<MessageInfo>>(
				QueryHelpers.AddQueryString("messages", query),
				cancellation);
		}

		[ItemCanBeNull]
		public Task<MessageInfo> GetMessageAsync(
			int messageID,
			bool? withRates = default,
			bool? withBodies = default,
			bool? formatBody = default,
			bool? withReadMarks = default,
			CancellationToken cancellation = default)
		{
			var query = new Dictionary<string, string>(3);
			query
				.AddToQuery(nameof(withRates), withRates)
				.AddToQuery(nameof(withBodies), withBodies)
				.AddToQuery(nameof(formatBody), formatBody)
				.AddToQuery(nameof(withReadMarks), withReadMarks);
			return GetAsync<MessageInfo>(
				QueryHelpers.AddQueryString($"messages/{messageID}", query),
				cancellation,
				true);
		}

		public async Task<MessageInfo> AddMessageAsync(AddMessageRequest request, CancellationToken cancellation = default) =>
			(await PostAsync<AddMessageRequest, MessageInfo>("messages", request, cancellation))
				.Result;
	}
}