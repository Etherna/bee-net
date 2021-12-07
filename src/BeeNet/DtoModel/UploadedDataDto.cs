namespace Etherna.BeeNet.DtoModel
{
    public class UploadedDataDto
    {
        public UploadedDataDto(string reference)
        {
            Reference = reference;
        }

        public string Reference { get; set; } = default!;
    }
}
