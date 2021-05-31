using Etherna.BeeNet.SampleClient.Operations;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Etherna.BeeNet.SampleClient
{
    class Program
    {
        // Enums.
        private enum UseMode { Download, Info, Stamps, Upload }

        // Consts.
        const string HelpText =
            "Bee.Net client sample help\n" +
            "Permits to get Bee node info, buy postage stamps, upload or download files.\n\n" +

            "Get node info:\n" +
            "./bee-sample.exe info [options]\n" +
            "\n" +

            "Buy postage stamps:\n" +
            "./bee-sample.exe stamps [options]\n" +
            "-a\tAmmount [default: 10000000]\n" +
            "-d\tBatch depth [default: 20]\n" +
            "\n" +

            "Upload command:\n" +
            "./bee-sample.exe upload <file> [options]\n" +
            "\n" +

            "Download command:\n" +
            "./bee-sample.exe download <hash> [options]\n" +
            "-o\tOutput file [default: ./output]\n" +
            "\n" +

            "Common options:\n" +
            "--gp\tGateway port [default: 1633]\n" +
            "--dp\tDebug port [default: 1635]\n" +
            "-u\tBee node Url [default: http://localhost]\n";

        [SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "CLI arguments don't have a name")]
        static async Task Main(string[] args)
        {
            UseMode mode;
            string? commandInput = null;
            var stampsAmmount = 10000000L;
            var batchDepth = 20;
            var outputFile = "./output";
            var baseUrl = "http://localhost";
            var debugPort = 1635;
            var gatewayPort = 1633;

            // Try to read arguments.
            try
            {
                if (args.Length == 0) throw new ArgumentException();
                int i = 0;

                //select mode
                mode = args[i++] switch
                {
                    "download" => UseMode.Download,
                    "info" => UseMode.Info,
                    "stamps" => UseMode.Stamps,
                    "upload" => UseMode.Upload,
                    _ => throw new ArgumentException(),
                };

                //read command input
                if (mode is UseMode.Download or UseMode.Upload)
                    commandInput = args[i++];

                //parse other arguments
                for (; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        //stamps
                        case "-a":
                            if (mode is not UseMode.Stamps)
                                throw new ArgumentException();
                            if (!long.TryParse(args[++i], out stampsAmmount))
                                throw new ArgumentException("Invalid stamps ammount");
                            break;

                        case "-d":
                            if (mode is not UseMode.Stamps)
                                throw new ArgumentException();
                            if (!int.TryParse(args[++i], out batchDepth))
                                throw new ArgumentException("Invalid batch depth");
                            break;

                        //download
                        case "-o":
                            if (mode is not UseMode.Download)
                                throw new ArgumentException();
                            outputFile = args[++i];
                            break;

                        //common
                        case "--dp":
                            if (args.Length <= i + 1)
                                throw new ArgumentException("A port is needed");
                            if (!int.TryParse(args[++i], out debugPort))
                                throw new ArgumentException("Invalid port");
                            break;

                        case "--gp":
                            if (args.Length <= i + 1)
                                throw new ArgumentException("A port is needed");
                            if (!int.TryParse(args[++i], out gatewayPort))
                                throw new ArgumentException("Invalid port");
                            break;

                        case "-u":
                            if (args.Length <= i + 1)
                                throw new ArgumentException("An url is needed");
                            baseUrl = args[++i];
                            break;

                        default: throw new ArgumentException();
                    }
                }
            }
            catch (ArgumentException)
            {
                Console.WriteLine(HelpText);
                return;
            }

            // Create client.
            var beeClient = new BeeNodeClient(baseUrl, gatewayPort, debugPort);

            // Execute command.
            switch (mode)
            {
                case UseMode.Info: await GetInfo.RunAsync(beeClient); break;
                case UseMode.Download: DownloadFile.Run(beeClient, commandInput!, outputFile); break;
                case UseMode.Stamps: await BuyStamps.RunAsync(beeClient, stampsAmmount, batchDepth); break;
                case UseMode.Upload: UploadFile.Run(beeClient, commandInput!); break;

                default: throw new NotSupportedException();
            }
        }
    }
}
