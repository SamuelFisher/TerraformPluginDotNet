using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TerraformPluginDotNet;

public static class HostApplicationLifetimeExtensions
{
    public static IHostApplicationLifetime InitializeTerraformPlugin(this IHostApplicationLifetime lifetime, IApplicationBuilder app, IOptions<TerraformPluginHostOptions> pluginHostOptions)
    {
        lifetime.ApplicationStarted.Register(() =>
        {
            var serverAddress = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses.First();
            var serverUri = new Uri(serverAddress);
            var host = serverUri.Host == "localhost" ? "127.0.0.1" : serverUri.Host;

            if (pluginHostOptions.Value.DebugMode)
            {
                Console.WriteLine("Debug mode enabled (no certificate). Run Terraform with the following environment variable set:");
                Console.WriteLine($@"TF_REATTACH_PROVIDERS={{""{pluginHostOptions.Value.FullProviderName}"":{{""Protocol"":""grpc"",""Pid"":{Environment.ProcessId},""Test"":true,""Addr"":{{""Network"":""tcp"",""String"":""{host}:{serverUri.Port}""}}}}}}");
            }
            else
            {
                var pluginHostCertificate = app.ApplicationServices.GetRequiredService<PluginHostCertificate>();
                Console.WriteLine($"1|5|tcp|{host}:{serverUri.Port}|grpc|{Convert.ToBase64String(pluginHostCertificate.Certificate.RawData)}");
            }
        });

        return lifetime;
    }
}
