using System;
using Xunit;

namespace BeeNet.IntegrationTest.BeeVersions
{
    public sealed class IgnoreOtherVersionFact : FactAttribute
    {
        public IgnoreOtherVersionFact(string testVersion)
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
