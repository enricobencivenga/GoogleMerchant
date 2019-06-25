using GoogleMerchant.Service;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace GoogleMerchant.Jobs
{
    class Program
    {
        private static readonly int MaxListPageSize = 50;


        [STAThread]
        internal static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfigurationRoot configuration = builder.Build();

            Console.WriteLine(configuration.GetConnectionString("Storage"));

            EntryService.Run(args);
        }
    }
}
