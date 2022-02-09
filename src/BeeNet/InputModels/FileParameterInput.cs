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
