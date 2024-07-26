using Etherna.BeeNet.Services;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace Etherna.BeeNet.Wasm
{
    [SuppressMessage("Design", "CA1052:Static holder types should be Static or NotInheritable")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public partial class BeeNetWasmUtil
    {
        public static void Main() { }
        
        [JSExport]
        internal static async Task<string> GetHashStringAsync(
            byte[] data,
            string fileContentType,
            string fileName)
        {
            var calculatorService = new CalculatorService();
            var result = await calculatorService.EvaluateFileUploadAsync(
                data,
                fileContentType,
                fileName).ConfigureAwait(false);
            return result.Hash.ToString();
        }
    }
}