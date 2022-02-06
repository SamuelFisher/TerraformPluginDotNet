using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TerraformPluginDotNet.ProviderConfig;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Serialization;
using TerraformPluginDotNet.Services;

namespace TerraformPluginDotNet;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddGrpc();

        services.AddTransient(typeof(ProviderConfigurationHost<>));
        services.AddTransient(typeof(ResourceProviderHost<>));
        services.AddTransient(typeof(IResourceUpgrader<>), typeof(DefaultResourceUpgrader<>));
        services.AddTransient<IDynamicValueSerializer, DefaultDynamicValueSerializer>();
    }

    public void Configure(
        IApplicationBuilder app,
        IHostApplicationLifetime lifetime,
        IWebHostEnvironment env,
        ILogger<Startup> logger,
        IOptions<TerraformPluginHostOptions> pluginHostOptions)
    {
        lifetime.ApplicationStarted.Register(() =>
        {
            logger.LogInformation("Application started.");

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

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapGrpcService<Terraform5ProviderService>();

            endpoints.MapGet("/", async context =>
            {
                await context.Response.WriteAsync("Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            });
        });
    }
}
