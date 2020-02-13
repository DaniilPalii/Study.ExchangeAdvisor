using ExchangeAdvisor.Domain.Services.Implementation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;

namespace ExchangeAdvisor.ML.SourceGenerator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var generatingFilePath = args.Length == 0 || string.IsNullOrWhiteSpace(args[0])
                ? DefaultFilePath
                : args[0];
            AssertThatPathIsValid(generatingFilePath);

            Console.Write("Generate file for Exchange Advisor neural network learning...");
            CreateFileWriter()
                .SaveAllExchangeRatesToTsv(generatingFilePath);
            Console.WriteLine("Done");

            Console.WriteLine($"File path: \"{generatingFilePath}\"");
            Console.WriteLine($"File size: {new FileSize(generatingFilePath)}");
            OpenFileWithDefaultProgram(generatingFilePath);

            Console.WriteLine("\nPress any key to exit...");
            Console.ReadKey();
        }

        private static void AssertThatPathIsValid(string generatingFilePath)
        {
            new FileInfo(generatingFilePath);
        }

        private static FileWriter CreateFileWriter()
        {
            var serviceProvider = new ServiceCollection().AddHttpClient().BuildServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var exchangeRateFetcher = new RateWebFetcher(httpClientFactory);

            return new FileWriter(exchangeRateFetcher);
        }

        private static void OpenFileWithDefaultProgram(string filePath)
        {
            var fileOpeningProcess = new Process
            {
                StartInfo = new ProcessStartInfo(filePath)
                {
                    UseShellExecute = true
                }
            };

            fileOpeningProcess.Start();
        }

        private const string DefaultFilePath = "Exchange rate history.tsv";
    }
}
