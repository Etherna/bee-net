// Copyright 2021-present Etherna SA
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Concurrent;

namespace Etherna.BeeNet.Hasher.Store
{
    public class MemoryStore : IStore
    {
        // Fields.
        private readonly ConcurrentDictionary<string, StoreItemBase> storeDictionary = new();

        // Methods.
        public bool TryGet(string storeKey, out StoreItemBase item) =>
            storeDictionary.TryGetValue(storeKey, out item!);

        public void Put(StoreItemBase item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            storeDictionary.TryAdd(item.StoreKey, item);
        }
    }
}