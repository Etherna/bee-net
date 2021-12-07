namespace Etherna.BeeNet.DtoModel
{
    public class StewardshipGetDto
    {
        public StewardshipGetDto(bool isRetrievable)
        {
            IsRetrievable = isRetrievable;
        }

        public bool IsRetrievable { get; set; } = default!;
    }
}
