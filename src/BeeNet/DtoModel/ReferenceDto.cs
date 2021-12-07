namespace Etherna.BeeNet.DtoModel
{
    public class ReferenceDto
    {
        public ReferenceDto(string reference)
        {
            Reference = reference;
        }

        public string Reference { get; set; } = default!;
    }
}
