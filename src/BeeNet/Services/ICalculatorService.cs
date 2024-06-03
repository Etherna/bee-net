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

using Etherna.BeeNet.Hasher.Postage;
using Etherna.BeeNet.Models;
using System.IO;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Services
{
    public interface ICalculatorService
    {
        /// <summary>
        /// Evaluate the result of upload a file
        /// </summary>
        /// <param name="stream">The file stream</param>
        /// <param name="contentType">The file content type</param>
        /// <param name="name">The file name</param>
        /// <param name="encrypt">True to encrypt</param>
        /// <param name="redundancyLevel">Choose the redundancy level</param>
        /// <param name="postageStampIssuer">Custom postage stamp issuer</param>
        /// <returns>The upload evaluation result</returns>
        Task<UploadEvaluationResult> EvaluateFileUploadAsync(
            Stream stream,
            string contentType,
            string? name,
            bool encrypt = false,
            RedundancyLevel redundancyLevel = RedundancyLevel.None,
            IPostageStampIssuer? postageStampIssuer = null);
    }
}