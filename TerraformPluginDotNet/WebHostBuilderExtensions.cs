using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TerraformPluginDotNet.ResourceProvider;

namespace TerraformPluginDotNet;

public static class WebHostBuilderExtensions
{
    public const int DefaultPort = 5344;

    public static IWebHostBuilder ConfigureTerraformPlugin(this IWebHostBuilder webBuilder, Action<IServiceCollection, ResourceRegistry> configureRegistry, int port = DefaultPort)
    {
        webBuilder.ConfigureKestrel(kestrel =>
        {
            var debugMode = kestrel.ApplicationServices.GetRequiredService<IOptions<TerraformPluginHostOptions>>().Value.DebugMode;

            if (debugMode)
            {
                kestrel.ListenLocalhost(port, x => x.Protocols = HttpProtocols.Http2);
            }
            else
            {
                kestrel.ListenLocalhost(port, x => x.UseHttps(x =>
                {
                    var certificate = kestrel.ApplicationServices.GetService<PluginHostCertificate>();
                    if (certificate == null)
                    {
                        throw new InvalidOperationException("Debug mode is not enabled, but no certificate was found.");
                    }

                    x.ServerCertificate = certificate.Certificate;
                    x.AllowAnyClientCertificate();
                }));
            }
        });

        webBuilder.UseStartup<Startup>();
        webBuilder.ConfigureServices(services =>
        {
            services.AddOptions<TerraformPluginHostOptions>().ValidateDataAnnotations();
            var registry = new ResourceRegistry();
            services.AddSingleton(registry);
            configureRegistry(services, registry);
        });

        return webBuilder;
    }
}
