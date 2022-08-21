using System;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions
{
    public sealed class IgnoreOtherVersionFactAttribute : FactAttribute
    {
        public IgnoreOtherVersionFactAttribute(string testVersion)
        {
            if (CurrentTestVersion(testVersion))
            {
                Skip = $"v. {testVersion} skip";
            }
        }

        private static bool CurrentTestVersion(string testVersion)
            => Environment.GetEnvironmentVariable("CurrentTestVersion") != testVersion;
    }
}
