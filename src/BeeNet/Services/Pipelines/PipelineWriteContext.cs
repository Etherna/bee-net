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

using System.Diagnostics.CodeAnalysis;

namespace Etherna.BeeNet.Services.Pipelines
{
    [SuppressMessage("Performance", "CA1819:Properties should not return arrays")]
    public class PipelineWriteContext
    {
        /// <summary>
        /// Reference, generated by bmt
        /// </summary>
        public byte[]? Reference { get; set; }
        
        /// <summary>
        /// Encryption key
        /// </summary>
        public byte[]? EncryptionKey { get; set; }
        
        /// <summary>
        /// Always unecrypted span uint64
        /// </summary>
        public byte[]? Span { get; set; }
        
        /// <summary>
        /// Data includes the span too, but it may be encrypted when the pipeline is encrypted
        /// </summary>
        public byte[]? Data { get; set; }
    }
}