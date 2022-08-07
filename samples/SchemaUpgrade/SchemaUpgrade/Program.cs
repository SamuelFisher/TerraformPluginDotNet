using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

namespace SchemaUpgrade;

class Program
{
    static Task Main(string[] args)
    {
        // Use the default plugin host that takes care of certificates and hosting the Grpc services.

        return TerraformPluginHost.RunAsync(args, "example.com/example/schemaupgrade", (services, registry) =>
        {
            // Register the resource and provider in the usual way.
            services.AddSingleton<IResourceProvider<UpgradableResourceV2>, UpgradableResourceProvider>();
            registry.RegisterResource<UpgradableResourceV2>("schemaupgrade_sampleresource");

            // Register the resource upgrader
            services.AddTransient<IResourceUpgrader<UpgradableResourceV2>, UpgradableResourceUpgrader>();
        });
    }
}
