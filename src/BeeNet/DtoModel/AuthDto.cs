namespace Etherna.BeeNet.DtoModel
{
    public class AuthDto
    {
        public AuthDto(string key)
        {
            Key = key;
        }

        public string Key { get; set; } = default!;
    }
}
