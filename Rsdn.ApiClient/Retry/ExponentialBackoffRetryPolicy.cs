using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Rsdn.ApiClient.Retry
{
	[PublicAPI]
	public abstract class ExponentialBackoffRetryPolicy : IRetryPolicy
	{
				/// <summary>
		/// Represents the default maximum amount of time used when calculating the exponential delay between retries.
		/// </summary>
		public TimeSpan MaxBackoff { get; }

		/// <summary>
		/// Represents the default minimum amount of time used when calculating the exponential delay between retries.
		/// </summary>
		public TimeSpan MinBackoff { get; }

		/// <summary>
		/// Represents the default amount of time used when calculating a random delta in the exponential delay between
		/// retries.
		/// </summary>
		public TimeSpan DeltaBackoff { get; }

		/// <summary>
		/// Initialize instance with specified retry count and backoff times.
		/// </summary>
		protected ExponentialBackoffRetryPolicy(
			int retryCount,
			TimeSpan maxBackoff,
			TimeSpan minBackoff,
			TimeSpan deltaBackoff)
		{
			RetryCount = retryCount;
			MaxBackoff = maxBackoff;
			MinBackoff = minBackoff;
			DeltaBackoff = deltaBackoff;
		}

		/// <summary>
		/// Initialize instance with specified retry count.
		/// </summary>
		protected ExponentialBackoffRetryPolicy(int retryCount)
			: this(
				retryCount,
				TimeSpan.FromSeconds(120),
				TimeSpan.FromSeconds(1),
				TimeSpan.FromSeconds(10))
		{ }

		/// <summary>
		/// Initialize instance with default params.
		/// </summary>
		protected ExponentialBackoffRetryPolicy() : this(3)
		{ }

		/// <summary>
		/// The number of retry attempts.
		/// </summary>
		public int RetryCount { get; }

		/// <summary>
		/// Returns true if exception is transient.
		/// </summary>
		protected abstract bool IsTransient(Exception exception);

		private TimeSpan CalculateBackoff(int retryCount)
		{
			var randomPart =
				new Random((int)(Stopwatch.GetTimestamp() >> 32))
					.Next(
						(int)(DeltaBackoff.TotalMilliseconds * 0.8),
						(int)(DeltaBackoff.TotalMilliseconds * 1.2));
			var mSecs =
				(int)Math.Min(
					MinBackoff.TotalMilliseconds + (int)((Math.Pow(2.0, retryCount) - 1.0) * randomPart),
					MaxBackoff.TotalMilliseconds);
			return TimeSpan.FromMilliseconds(mSecs);
		}

		/// <inheritdoc />
		public async Task<T> ExecuteAsync<T>(
			Func<CancellationToken, Task<T>> action,
			CancellationToken cancellation = default)
		{
			var exceptions = new List<Exception>();

			while (true)
			{
				cancellation.ThrowIfCancellationRequested();

				try
				{
					var result = await action(cancellation).ConfigureAwait(false);
					return result;
				}
				catch (Exception ex)
				{
					if (!IsTransient(ex))
						throw;

					exceptions.Add(ex);
					if (exceptions.Count >= RetryCount)
						throw new AggregateException(exceptions);

					await Task.Delay(CalculateBackoff(exceptions.Count), cancellation).ConfigureAwait(false);
				}
			}
		}
	}
}