using Etherna.BeeNet.SampleClient.Operations;
using System;

namespace Etherna.BeeNet.SampleClient
{
    class Program
    {
        // Consts.
        const string HelpText =
            "Bee.Net client sample help:\n" +
            "Upload or download a file with a Bee node instance.\n\n" +

            "Download command:\n" +
            "./bee-sample.exe download <hash> [options]\n" +
            "-o\tOutput file [default: ./output]\n" +
            "\n" +

            "Upload command:\n" +
            "./bee-sample.exe upload <file> [options]\n" +
            "\n" +

            "Common options:\n" +
            "-u\tBee node Url [default: http://localhost]\n" +
            "-p\tGateway port [default: 1633]\n";

        static void Main(string[] args)
        {
            string? mode;
            string? commandInput;
            var outputFile = "./output";
            var baseUrl = "http://localhost";
            var port = 1633;

            // Try to read arguments.
            try
            {
                if (args.Length < 2) throw new InvalidOperationException();

                //select mode
                mode = args[0] switch
                {
                    "download" or "upload" => args[0],
                    _ => throw new InvalidOperationException(),
                };

                //read command input
                commandInput = args[1];

                //parse other arguments
                for (int i = 2; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "-o":
                            if (mode != "download")
                                throw new InvalidOperationException();
                            outputFile = args[++i];
                            break;

                        case "-p":
                            if (!int.TryParse(args[++i], out port))
                                throw new InvalidOperationException();
                            break;

                        case "-u":
                            baseUrl = args[++i];
                            if (string.IsNullOrEmpty(baseUrl))
                                throw new InvalidOperationException();
                            break;

                        default: throw new InvalidOperationException();
                    }
                }
            }
            catch (InvalidOperationException)
            {
                Console.WriteLine(HelpText);
                return;
            }

            // Create client.
            var beeClient = new BeeNetClient(baseUrl, gatewayApiPort: port);

            // Execute command.
            switch (mode)
            {
                case "download": DownloadFile.Run(beeClient, commandInput, outputFile); break;
                case "upload": UploadFile.Run(beeClient, commandInput); break;

                default: throw new NotSupportedException();
            }
        }
    }
}
