using Etherna.BeeNet.Models;
using System.Collections.Generic;

namespace Etherna.BeeNet.Manifest
{
    public interface IReadOnlyMantarayNode
    {
        // Properties.
        SwarmHash? EntryHash { get; }
        SwarmHash Hash { get; }
        IReadOnlyDictionary<string, string> Metadata { get; }
        NodeType NodeTypeFlags { get; }
        XorEncryptKey? ObfuscationKey { get; }
    }
}