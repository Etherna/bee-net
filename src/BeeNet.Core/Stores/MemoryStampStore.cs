// Copyright 2021-present Etherna SA
// This file is part of Bee.Net.
// 
// Bee.Net is free software: you can redistribute it and/or modify it under the terms of the
// GNU Lesser General Public License as published by the Free Software Foundation,
// either version 3 of the License, or (at your option) any later version.
// 
// Bee.Net is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License along with Bee.Net.
// If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Etherna.BeeNet.Stores
{
    public class MemoryStampStore : IStampStore
    {
        // Fields.
        private readonly ConcurrentDictionary<string, StampStoreItem> storeDictionary = new();

        // Constructor.
        public MemoryStampStore(IEnumerable<StampStoreItem>? initialItems = null)
        {
            foreach (var item in initialItems ?? [])
                storeDictionary.TryAdd(item.Id, item);
        }

        // Methods.
        public IEnumerable<StampStoreItem> GetItems() =>
            storeDictionary.Values;

        public void Put(StampStoreItem item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            storeDictionary.TryAdd(item.Id, item);
        }
        
        public bool TryGet(string storeKey, out StampStoreItem item) =>
            storeDictionary.TryGetValue(storeKey, out item!);
    }
}