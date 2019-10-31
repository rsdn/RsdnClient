using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Rsdn.Api.Models.Errors;

namespace Rsdn.ApiClient
{
	[PublicAPI]
	public abstract class ApiBase
	{
		private readonly TokenFactory _tokenFactory;

		protected ApiBase(
			HttpClient httpClient,
			TokenFactory tokenFactory,
			JsonSerializerOptions serializerOptions)
		{
			_tokenFactory = tokenFactory;
			HttpClient = httpClient;
			SerializerOptions = serializerOptions;
		}

		public JsonSerializerOptions SerializerOptions { get; }
		protected HttpClient HttpClient { get; }

		protected async Task<TResult> GetAsync<TResult>(string uri, CancellationToken cancellation, bool defaultOnNotFound = false)
		{
			var request = new HttpRequestMessage(HttpMethod.Get, uri);
			var token = await _tokenFactory(cancellation);
			if (token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await HttpClient.SendAsync(request, cancellation);
			return await ProcessResponseAsync<TResult>(response, defaultOnNotFound);
		}

		protected async Task DeleteAsync(
			string uri,
			CancellationToken cancellation)
		{
			var request = new HttpRequestMessage(HttpMethod.Delete, uri);
			var token = await _tokenFactory(cancellation);
			if (token != null)
				request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await HttpClient.SendAsync(request, cancellation);
			await ProcessResponseAsync(response);
		}

		protected async Task<PostResult<TResult>> PostAsync<TRequest, TResult>(
			string uri,
			TRequest request,
			CancellationToken cancellation)
		{
			var httpRequest = new HttpRequestMessage(HttpMethod.Post, uri)
			{
				// Add request id header for idempotent POST method
				Headers = { {"X-Request-ID", new []{Guid.NewGuid().ToString("N")}} },
				Content =
					new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(request))
					{
						Headers = { ContentType = new MediaTypeHeaderValue("application/json") }
					}
			};
			var token = await _tokenFactory(cancellation);
			if (token != null)
				httpRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			var response = await HttpClient.SendAsync(httpRequest, cancellation);

			TResult result = default;
			if (typeof(TResult) == typeof(EmptyResponse))
				await ProcessResponseAsync(response);
			else
				result = await ProcessResponseAsync<TResult>(response, false);
			return new PostResult<TResult>(result, response.Headers.Location);
		}

		private async Task<T> ProcessResponseAsync<T>(HttpResponseMessage response, bool defaultOnNotFound)
		{
			var content = await response.Content.ReadAsStringAsync();
			if (response.IsSuccessStatusCode)
				return JsonSerializer.Deserialize<T>(content, SerializerOptions);
			if (defaultOnNotFound && response.StatusCode == HttpStatusCode.NotFound)
				return default;
			RsdnServiceError error;
			try
			{
				error = JsonSerializer.Deserialize<RsdnServiceError>(content);
			}
			catch
			{
				error = null;
			}
			throw new RsdnServiceException(
				new ErrorType(
					response.StatusCode,
					error != null ? error.Type : response.StatusCode.ToString()), 
				error?.Severity ?? ErrorSeverity.Error,
				error != null
					? error.Title
					: $"Server error: {response.StatusCode}",
				error != null ? error.Detail : content);
		}

		private async Task ProcessResponseAsync(HttpResponseMessage response)
		{
			if (response.IsSuccessStatusCode)
				return;
			var content = await response.Content.ReadAsStringAsync();
			RsdnServiceError error;
			try
			{
				error = JsonSerializer.Deserialize<RsdnServiceError>(content);
			}
			catch
			{
				error = null;
			}
			throw new RsdnServiceException(
				new ErrorType(
					response.StatusCode,
					error != null ? error.Type : response.StatusCode.ToString()), 
				error?.Severity ?? ErrorSeverity.Error,
				error != null
					? error.Title
					: $"Server error: {response.StatusCode}",
				error != null ? error.Detail : content);
		}

		[PublicAPI]
		protected class PostResult<TResult>
		{
			public TResult Result { get; }
			public Uri Location { get; }

			public PostResult(TResult result, Uri location)
			{
				Result = result;
				Location = location;
			}
		}

		protected class EmptyResponse {}
	}
}