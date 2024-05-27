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

namespace Etherna.BeeNet.Services.Store
{
    public interface IStore
    {
        /// <summary>
        /// Unmarshalls object with the given Item.Key.ID into the given Item.
        /// </summary>
        /// <param name="item">Item to get</param>
        /// <returns>True if item found</returns>
        public bool TryGet(StoreItemBase item);

        /// <summary>
        /// Inserts or updates the given Item identified by its Key.ID.
        /// </summary>
        /// <param name="item">Item to put</param>
        public void Put(StoreItemBase item);
    }
}