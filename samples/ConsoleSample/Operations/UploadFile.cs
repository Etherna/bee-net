using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Etherna.BeeNet.SampleClient.Operations
{
    class UploadFile
    {
        internal static void Run(BeeNodeClient beeClient, string inputFile)
        {
            // Get file.
            var fileStream = File.OpenRead(inputFile);

            // Get available postage stamps.

            // Upload file.

        }
    }
}
