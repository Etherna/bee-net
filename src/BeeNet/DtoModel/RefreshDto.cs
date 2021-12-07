namespace Etherna.BeeNet.DtoModel
{
    public class RefreshDto
    {
        public RefreshDto(string key)
        {
            Key = key;
        }

        public string Key { get; set; } = default!;
    }
}
