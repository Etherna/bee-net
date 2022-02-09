//   Copyright 2021-present Etherna Sagl
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

using System.IO;

namespace Etherna.BeeNet.InputModels
{
    public class FileParameterInput
    {
        // Constructors.
        public FileParameterInput(Stream data, string? fileName = null, string? contentType = null)
        {
            Data = data;
            FileName = fileName;
            ContentType = contentType;
        }

        // Properties.
        public Stream Data { get; private set; }
        public string? FileName { get; private set; }
        public string? ContentType { get; private set; }
    }
}
