using System;
using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet.ResourceProvider;

namespace SampleProvider
{
    class Program
    {
        static void Main(string[] args)
        {
            TerraformPluginDotNet.Program.Run(args, (services, registry) =>
            {
                services.AddSingleton<IResourceProvider<SampleFileResource>, SampleFileResourceProvider>();
                registry.RegisterResource<SampleFileResource>("dotnetsample_file");
            });
        }
    }
}
