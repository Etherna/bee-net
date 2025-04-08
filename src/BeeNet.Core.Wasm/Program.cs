using Etherna.BeeNet.Hashing;
using Etherna.BeeNet.Models;
using Etherna.BeeNet.Services;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

#pragma warning disable CA1303
Console.WriteLine("Main, don't remove!");
#pragma warning restore CA1303

[SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("Design", "CA1050:Declare types in namespaces")]
#pragma warning disable CA1515
public partial class BeeNetWasmUtil
{
    [JSExport]
    internal static async Task<string> GetHashStringAsync(
        byte[] data,
        string fileContentType,
        string fileName)
    {
        var calculatorService = new ChunkService();
        var result = await calculatorService.UploadSingleFileAsync(
            data,
            fileContentType,
            fileName,
            () => new SwarmChunkBmt()).ConfigureAwait(false);
        return result.ChunkReference.Hash.ToString();
    }
}
#pragma warning restore CA1515
