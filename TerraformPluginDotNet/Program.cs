using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformPluginDotNet
{
    public class Program
    {
        public static X509Certificate2 Cert { get; private set; }

        public static void Run(string[] args, Action<IServiceCollection, ResourceRegistry> configure)
        {
            Cert = CertificateGenerator.GenerateSelfSignedCertificate("CN=127.0.0.1", "CN=root ca", CertificateGenerator.GeneratePrivateKey());

            var serilogConfiguration = new ConfigurationBuilder()
                .AddJsonFile("serilog.json", optional: true)
                .Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(serilogConfiguration)
                .CreateLogger();

            try
            {
                CreateHostBuilder(args, configure).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Logger.Fatal(ex, "Fatal error occurred.");
            }
            finally
            {
                Log.Logger.Information("Application terminated.");
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args, Action<IServiceCollection, ResourceRegistry> configure) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(x =>
                    {
                        x.ListenLocalhost(5344, x => x.UseHttps(x =>
                        {
                            x.ServerCertificate = Cert;
                            x.AllowAnyClientCertificate();
                        }));
                    });
                    webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureLogging((_, x) => x.ClearProviders().AddSerilog());
                    webBuilder.ConfigureServices(services =>
                    {
                        var registry = new ResourceRegistry();
                        services.AddSingleton(registry);
                        configure(services, registry);
                    });
                });
    }
}
