using System;
using System.Threading;
using System.Threading.Tasks;

namespace Rsdn.ApiClient.Retry
{
	/// <summary>
	/// Retry policy interface.
	/// </summary>
	public interface IRetryPolicy
	{
		/// <summary>
		/// Execute <paramref name="function"/> asynchronously with retry policy.
		/// </summary>
		Task<TResult> ExecuteAsync<TResult>(
			Func<CancellationToken, Task<TResult>> function,
			CancellationToken cancellation = default);
	}
}