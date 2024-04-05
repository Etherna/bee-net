﻿//   Copyright 2021-present Etherna SA
// 
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
// 
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Globalization;
using System.Text.Json;

namespace Etherna.BeeNet.Models
{
    public sealed class PostageBatch
    {
        // Constructors.
        internal PostageBatch(Clients.DebugApi.Response39 batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));

            AmountPaid = batch.Amount is null ? null : long.Parse(batch.Amount, CultureInfo.InvariantCulture);
            BatchTTL = batch.BatchTTL;
            Exists = batch.Exists;
            Id = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        internal PostageBatch(Clients.DebugApi.Stamps batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));

            AmountPaid = batch.Amount is null ? null : long.Parse(batch.Amount, CultureInfo.InvariantCulture);
            BatchTTL = batch.BatchTTL;
            Exists = batch.Exists;
            Id = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        internal PostageBatch(Clients.GatewayApi.Stamps batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));

            AmountPaid = batch.Amount is null ? null : long.Parse(batch.Amount, CultureInfo.InvariantCulture);
            BatchTTL = batch.BatchTTL;
            Exists = batch.Exists;
            Id = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        internal PostageBatch(Clients.GatewayApi.Response52 batch)
        {
            ArgumentNullException.ThrowIfNull(batch, nameof(batch));

            AmountPaid = batch.Amount is null ? null : long.Parse(batch.Amount, CultureInfo.InvariantCulture);
            BatchTTL = batch.BatchTTL;
            Exists = batch.Exists;
            Id = batch.BatchID;
            Utilization = batch.Utilization;
            Usable = batch.Usable;
            Label = batch.Label;
            Depth = batch.Depth;
            BucketDepth = batch.BucketDepth;
            BlockNumber = batch.BlockNumber;
            ImmutableFlag = batch.ImmutableFlag;
        }

        // Properties.
        public string Id { get; }
        public long? AmountPaid { get; }
        /// <summary>The time (in seconds) remaining until the batch expires; -1 signals that the batch never expires; 0 signals that the batch has already expired.</summary>
        public long BatchTTL { get; }
        public int BlockNumber { get; }
        public int BucketDepth { get; }
        public int Depth { get; }
        public bool Exists { get; }
        public bool ImmutableFlag { get; }
        public string? Label { get; }
        /// <summary>Indicate that the batch was discovered by the Bee node, but it awaits enough on-chain confirmations before declaring the batch as usable.</summary>
        public bool Usable { get; }
        public int? Utilization { get; }
    }

}