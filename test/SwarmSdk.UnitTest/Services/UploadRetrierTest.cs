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
using Microsoft.Extensions.Time.Testing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Etherna.SwarmSdk.Services
{
    public class UploadRetrierTest
    {
        // Consts.
        private static readonly TimeSpan BaseDelay = TimeSpan.FromSeconds(1);

        // Tests.
        [Theory]
        [InlineData(408, true)]
        [InlineData(429, true)]
        [InlineData(500, true)]
        [InlineData(502, true)]
        [InlineData(503, true)]
        [InlineData(504, true)]
        [InlineData(400, false)]
        [InlineData(402, false)]
        [InlineData(404, false)]
        [InlineData(413, false)]
        public void ClassifiesApiStatusCodesAsTransientOrNot(int statusCode, bool expectedTransient)
        {
            Assert.Equal(expectedTransient, UploadRetrier.IsTransientError(ApiException(statusCode)));
        }

        [Fact]
        public void ClassifiesNetworkFaultsAsTransientAndOthersNot()
        {
            Assert.True(UploadRetrier.IsTransientError(new HttpRequestException()));
            Assert.True(UploadRetrier.IsTransientError(new IOException()));
            Assert.True(UploadRetrier.IsTransientError(new TimeoutException()));
            Assert.False(UploadRetrier.IsTransientError(new InvalidOperationException()));
        }

        [Fact]
        public async Task DoesNotRetryNonTransientError()
        {
            // Setup.
            var callCount = 0;
            Task<int> Action(CancellationToken _)
            {
                callCount++;
                throw ApiException(400);
            }

            // Run & Assert.
            await Assert.ThrowsAsync<SwarmSdkApiException>(() =>
                UploadRetrier.ExecuteWithRetryAsync(Action, baseDelay: BaseDelay, timeProvider: new FakeTimeProvider()));
            Assert.Equal(1, callCount); // non-transient failures abort immediately
        }

        [Fact]
        public async Task DoesNotRunActionWhenCancellationAlreadyRequested()
        {
            // Setup.
            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();
            var callCount = 0;
            Task<int> Action(CancellationToken _)
            {
                callCount++;
                return Task.FromResult(1);
            }

            // Run & Assert.
            await Assert.ThrowsAsync<OperationCanceledException>(() =>
                UploadRetrier.ExecuteWithRetryAsync(
                    Action, timeProvider: new FakeTimeProvider(), cancellationToken: cts.Token));
            Assert.Equal(0, callCount);
        }

        [Fact]
        public async Task RetriesTransientFailuresThenSucceeds()
        {
            // Setup.
            // The first two attempts fail with transient errors, the third succeeds: a virtual clock
            // releases each backoff delay so the staged outcome is observed deterministically.
            var fakeTimeProvider = new FakeTimeProvider();
            var callCount = 0;
            Task<int> Action(CancellationToken _)
            {
                callCount++;
                if (callCount < 3)
                    throw ApiException(503);
                return Task.FromResult(7);
            }

            // Run.
            var task = UploadRetrier.ExecuteWithRetryAsync(Action, baseDelay: BaseDelay, timeProvider: fakeTimeProvider);
            await DriveVirtualClockUntilCompletedAsync(fakeTimeProvider, task, BaseDelay);
            var result = await task;

            // Assert.
            Assert.Equal(7, result);
            Assert.Equal(3, callCount);
        }

        [Fact]
        public async Task ReturnsResultWhenActionSucceedsImmediately()
        {
            // Setup.
            var callCount = 0;
            Task<int> Action(CancellationToken _)
            {
                callCount++;
                return Task.FromResult(42);
            }

            // Run.
            var result = await UploadRetrier.ExecuteWithRetryAsync(Action, timeProvider: new FakeTimeProvider());

            // Assert.
            Assert.Equal(42, result);
            Assert.Equal(1, callCount); // no retry needed on success
        }

        [Fact]
        public async Task ThrowsLastErrorAfterExhaustingAttempts()
        {
            // Setup.
            var fakeTimeProvider = new FakeTimeProvider();
            var callCount = 0;
            Task<int> Action(CancellationToken _)
            {
                callCount++;
                throw ApiException(500);
            }

            // Run.
            var task = UploadRetrier.ExecuteWithRetryAsync(
                Action, maxAttempts: 3, baseDelay: BaseDelay, timeProvider: fakeTimeProvider);
            await DriveVirtualClockUntilCompletedAsync(fakeTimeProvider, task, BaseDelay);

            // Assert.
            await Assert.ThrowsAsync<SwarmSdkApiException>(() => task);
            Assert.Equal(3, callCount); // initial try plus two retries
        }

        [Fact]
        public async Task ThrowsOnInvalidMaxAttempts()
        {
            // Setup.
            Task<int> Action(CancellationToken _) => Task.FromResult(1);

            // Run & Assert.
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>
                UploadRetrier.ExecuteWithRetryAsync(Action, maxAttempts: 0, timeProvider: new FakeTimeProvider()));
        }

        [Fact]
        public async Task VoidOverloadRetriesTransientThenCompletes()
        {
            // Setup.
            var fakeTimeProvider = new FakeTimeProvider();
            var callCount = 0;
            Task Action(CancellationToken _)
            {
                callCount++;
                if (callCount < 2)
                    throw new HttpRequestException("transient");
                return Task.CompletedTask;
            }

            // Run.
            var task = UploadRetrier.ExecuteWithRetryAsync(Action, baseDelay: BaseDelay, timeProvider: fakeTimeProvider);
            await DriveVirtualClockUntilCompletedAsync(fakeTimeProvider, task, BaseDelay);
            await task;

            // Assert.
            Assert.Equal(2, callCount);
        }

        // Helpers.
        private static SwarmSdkApiException ApiException(int statusCode) =>
            new("error", statusCode, null, new Dictionary<string, IEnumerable<string>>(), null);

        /// <summary>
        /// Repeatedly advances the virtual clock by <paramref name="step"/> until the retrying task
        /// completes, settling the async continuations between advances. Bounded by wall-clock time
        /// so a stuck task cannot hang the test.
        /// </summary>
        private static async Task DriveVirtualClockUntilCompletedAsync(
            FakeTimeProvider timeProvider,
            Task task,
            TimeSpan step)
        {
            var realTimeout = Stopwatch.StartNew();
            while (!task.IsCompleted && realTimeout.Elapsed < TimeSpan.FromSeconds(5))
            {
                timeProvider.Advance(step);
                await Task.Delay(1);
            }
        }
    }
}
