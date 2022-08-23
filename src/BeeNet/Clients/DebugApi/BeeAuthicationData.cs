using System;

namespace Etherna.BeeNet.Clients.DebugApi
{
    public class BeeAuthicationData
    {
        public BeeAuthicationData(
            string username,
            string password)
        {
            if (username is null)
                throw new ArgumentNullException(nameof(username));
            if (password is null)
                throw new ArgumentNullException(nameof(password));

            Username = username;
            Password = password;
        }

        public string Username { get; set; }
        public string Password { get; set; }
    }
}
