using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

namespace SampleProvider;

class Program
{
    static Task Main(string[] args)
    {
        // Use the default plugin host that takes care of certificates and hosting the Grpc services.

        return TerraformPluginHost.RunAsync(args, "example.com/example/dotnetsample", (services, registry) =>
        {
            services.AddSingleton<SampleConfigurator>();
            services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>();
            services.AddSingleton<IResourceProvider<SampleFileResource>, SampleFileResourceProvider>();
            registry.RegisterResource<SampleFileResource>("dotnetsample_file");
        });
    }
}
