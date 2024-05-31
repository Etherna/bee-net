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

namespace Etherna.BeeNet.Store
{
    public class MemoryStore : IStore
    {
        private ConcurrentDictionary<string, byte[]> storeDictionary = new();
        
        public bool TryGet(StoreItemBase item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));

            if (!storeDictionary.TryGetValue(Key(item), out var value))
                return false;
            
            item.Unmarshal(value);
            return true;
        }

        public void Put(StoreItemBase item)
        {
            ArgumentNullException.ThrowIfNull(item, nameof(item));
            var val = item.Marshal();
            storeDictionary.TryAdd(Key(item), val);
        }
        
        // Helpers.
        private static string Key(StoreItemBase item) =>
            string.Join("/", item.NamespaceStr, item.Id);
    }
}