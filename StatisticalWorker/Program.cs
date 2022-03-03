using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using StatisticalWorker.Workers;

namespace StatisticalWorker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .ConfigureAppConfiguration((hostContext, builder) =>
                        {
                            builder
                                .SetBasePath(Directory.GetCurrentDirectory())
                                .AddEnvironmentVariables()
                                .AddJsonFile($"appsettings.json", false, true)
                                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", false, true);
                            hostContext.Configuration = builder.Build();
                            builder.AddJsonStream(GetRemoteConfig(hostContext.Configuration.GetSection("ConfigService").Get<string[]>()));
                        })
                        .UseSerilog((hostContext, loggerConfiguration) =>
                        {
                            loggerConfiguration
                                .ReadFrom.Configuration(hostContext.Configuration)
                                .Enrich.FromLogContext()
                                .Enrich.WithExceptionDetails();
                        })
                        .UseStartup<Startup>();
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<LeaderBoardWorker>();
                    services.AddHostedService<OnlineCountWorker>();
                    services.AddHostedService<TpBetRecordRelayForReportWorker>();
                    services.AddHostedService<CustClosingBalanceWorker>();
                    if (hostContext.Configuration.GetSection("RunCustomerWorker").Get<bool>())
                    {
                        services.AddHostedService<CustomerWorker>();
                    }
                });
        
        private static MemoryStream GetRemoteConfig(IEnumerable<string> remoteUrls)
        {
            using var client = new HttpClient();
            Stream result = null;
            foreach (var url in remoteUrls)
            {
                if (string.IsNullOrEmpty(url))
                {
                    continue;
                }

                try
                {
                    result = client.GetAsync(url).Result.Content.ReadAsStreamAsync().Result;
                    break;
                }
                catch { }
            }

            using var streamReader = new StreamReader(result);
            return new MemoryStream(Encoding.UTF8.GetBytes(streamReader.ReadToEnd()));
        }
    }
}
