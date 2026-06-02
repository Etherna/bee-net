// Copyright 2021-present Etherna SA
// This file is part of SwarmSDK.
//
// SwarmSDK is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
//
// SwarmSDK is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along with SwarmSDK.
// If not, see <https://www.gnu.org/licenses/>.

using Etherna.SwarmSdk.Exceptions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Etherna.SwarmSdk.Services
{
    /// <summary>
    /// Retries an upload action on transient failures, using exponential backoff.
    /// </summary>
    /// <remarks>
    /// Retrying an upload that reuses the same postage batch is safe: chunks are content-addressed and
    /// the postage stamp for a given chunk address always maps to the same bucket, so re-sending an
    /// already-computed chunk set neither consumes additional batch utilization nor creates new content.
    /// This holds for the network push of a materialized chunk set; it does NOT hold when each attempt
    /// re-encrypts the content with a fresh random key, because that produces different chunk addresses.
    /// </remarks>
    public static class UploadRetrier
    {
        // Consts.
        /// <summary>Default base delay before the first retry; grows exponentially per attempt up to <see cref="DefaultMaxDelay"/>.</summary>
        public static readonly TimeSpan DefaultBaseDelay = TimeSpan.FromSeconds(2);

        /// <summary>Default maximum number of attempts (the initial try plus retries).</summary>
        public const int DefaultMaxAttempts = 5;

        /// <summary>Default upper bound for the delay between retries (cap of the exponential backoff).</summary>
        public static readonly TimeSpan DefaultMaxDelay = TimeSpan.FromSeconds(30);

        // Static methods.
        /// <summary>
        /// Runs an upload action, retrying it on transient failures with exponential backoff.
        /// </summary>
        /// <typeparam name="T">Type of the upload result.</typeparam>
        /// <param name="uploadAction">
        /// The upload to run. It must be repeatable: any stream it reads has to be rewindable or rebuilt on each
        /// invocation, otherwise a retry would resend an already-consumed payload.
        /// </param>
        /// <param name="maxAttempts">Maximum number of attempts (initial try plus retries). 1 disables retrying. Defaults to <see cref="DefaultMaxAttempts"/>.</param>
        /// <param name="baseDelay">Delay before the first retry, doubled on each subsequent retry. Defaults to <see cref="DefaultBaseDelay"/>.</param>
        /// <param name="maxDelay">Upper bound for the backoff delay. Defaults to <see cref="DefaultMaxDelay"/>.</param>
        /// <param name="isTransient">Predicate deciding whether a failure is transient and worth retrying. Defaults to <see cref="IsTransientError"/>.</param>
        /// <param name="timeProvider">Time source driving the backoff delays. Defaults to <see cref="TimeProvider.System"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the first successful attempt.</returns>
        [SuppressMessage("Design", "CA1031:Do not catch general exception types",
            Justification = "The transient-failure predicate decides which exception types are retried; non-transient ones are rethrown by the filter.")]
        public static async Task<T> ExecuteWithRetryAsync<T>(
            Func<CancellationToken, Task<T>> uploadAction,
            int maxAttempts = DefaultMaxAttempts,
            TimeSpan? baseDelay = null,
            TimeSpan? maxDelay = null,
            Func<Exception, bool>? isTransient = null,
            TimeProvider? timeProvider = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(uploadAction);

            var effectiveBaseDelay = baseDelay ?? DefaultBaseDelay;
            var effectiveMaxDelay = maxDelay ?? DefaultMaxDelay;
            var effectiveIsTransient = isTransient ?? IsTransientError;
            var effectiveTimeProvider = timeProvider ?? TimeProvider.System;
            if (maxAttempts < 1)
                throw new ArgumentOutOfRangeException(nameof(maxAttempts), maxAttempts, "Max attempts must be at least 1");
            if (effectiveBaseDelay <= TimeSpan.Zero)
                throw new ArgumentOutOfRangeException(nameof(baseDelay), effectiveBaseDelay, "Base delay must be positive");
            if (effectiveMaxDelay < effectiveBaseDelay)
                throw new ArgumentOutOfRangeException(nameof(maxDelay), effectiveMaxDelay, "Max delay cannot be less than base delay");

            for (var attempt = 1; ; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    return await uploadAction(cancellationToken).ConfigureAwait(false);
                }
                catch (Exception e) when (
                    attempt < maxAttempts &&
                    !(e is OperationCanceledException && cancellationToken.IsCancellationRequested) &&
                    effectiveIsTransient(e))
                {
                    // Transient failure with attempts left: back off, then retry.
                    var delay = ComputeBackoffDelay(attempt, effectiveBaseDelay, effectiveMaxDelay);
                    await Task.Delay(delay, effectiveTimeProvider, cancellationToken).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// Runs an upload action with no result, retrying it on transient failures with exponential backoff.
        /// </summary>
        /// <param name="uploadAction">
        /// The upload to run. It must be repeatable: any stream it reads has to be rewindable or rebuilt on each
        /// invocation, otherwise a retry would resend an already-consumed payload.
        /// </param>
        /// <param name="maxAttempts">Maximum number of attempts (initial try plus retries). 1 disables retrying. Defaults to <see cref="DefaultMaxAttempts"/>.</param>
        /// <param name="baseDelay">Delay before the first retry, doubled on each subsequent retry. Defaults to <see cref="DefaultBaseDelay"/>.</param>
        /// <param name="maxDelay">Upper bound for the backoff delay. Defaults to <see cref="DefaultMaxDelay"/>.</param>
        /// <param name="isTransient">Predicate deciding whether a failure is transient and worth retrying. Defaults to <see cref="IsTransientError"/>.</param>
        /// <param name="timeProvider">Time source driving the backoff delays. Defaults to <see cref="TimeProvider.System"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public static Task ExecuteWithRetryAsync(
            Func<CancellationToken, Task> uploadAction,
            int maxAttempts = DefaultMaxAttempts,
            TimeSpan? baseDelay = null,
            TimeSpan? maxDelay = null,
            Func<Exception, bool>? isTransient = null,
            TimeProvider? timeProvider = null,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(uploadAction);

            return ExecuteWithRetryAsync(
                async ct =>
                {
                    await uploadAction(ct).ConfigureAwait(false);
                    return true;
                },
                maxAttempts,
                baseDelay,
                maxDelay,
                isTransient,
                timeProvider,
                cancellationToken);
        }

        /// <summary>
        /// Determines whether an exception is a transient upload failure worth retrying.
        /// Covers server-side errors (HTTP 408/429/500/502/503/504) and network-level faults.
        /// </summary>
        /// <param name="exception">The exception to classify.</param>
        /// <returns>True if the failure looks transient; otherwise false.</returns>
        public static bool IsTransientError(Exception exception)
        {
            ArgumentNullException.ThrowIfNull(exception);

            return exception switch
            {
                SwarmSdkApiException apiException =>
                    apiException.StatusCode is 408 or 429 or 500 or 502 or 503 or 504,
                HttpRequestException => true,
                WebSocketException => true,
                SocketException => true,
                IOException => true,
                TimeoutException => true,
                _ => false
            };
        }

        // Helpers.
        private static TimeSpan ComputeBackoffDelay(int attempt, TimeSpan baseDelay, TimeSpan maxDelay)
        {
            // Exponential backoff: baseDelay * 2^(attempt-1), capped at maxDelay.
            // The multiplier is computed in double to avoid Int64 overflow before the cap is applied.
            var scaledTicks = baseDelay.Ticks * Math.Pow(2, attempt - 1);
            return scaledTicks >= maxDelay.Ticks
                ? maxDelay
                : TimeSpan.FromTicks((long)scaledTicks);
        }
    }
}
