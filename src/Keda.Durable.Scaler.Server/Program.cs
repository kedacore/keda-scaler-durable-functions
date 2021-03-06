using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Keda.Durable.Scaler.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        // Additional configuration is required to successfully run gRPC on macOS.
        // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.AddConsole();
                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options =>
                    {
                        options.Listen(IPAddress.Any, 5000, o => o.Protocols = HttpProtocols.Http2);
                        //options.Listen(IPAddress.Any, 5001, listenOptions =>
                        //{
                        //    listenOptions.Protocols = HttpProtocols.Http2;
                        //    // listenOptions.UseHttps(Environment.GetEnvironmentVariable(Startup.CertPath),
                        //    //    Environment.GetEnvironmentVariable(Startup.CertPass));
                        //});
                    });
                    webBuilder.UseStartup<Startup>();
                });
    }
}
