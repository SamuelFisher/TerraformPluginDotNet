﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet;
using TerraformPluginDotNet.ResourceProvider;

namespace DataSourceProvider;

class Program
{
    static Task Main(string[] args)
    {
        // Use the default plugin host that takes care of certificates and hosting the Grpc services.

        return TerraformPluginHost.RunAsync(args, "example.com/example/datasourceprovider", (services, registry) =>
        {
            services.AddSingleton<SampleConfigurator>();
            services.AddTerraformProviderConfigurator<Configuration, SampleConfigurator>();
            services.AddSingleton<IDataSourceProvider<SampleDataSource>, SampleDataSourceProvider>();
            registry.RegisterDataSource<SampleDataSource>("datasourceprovider_data");
        });
    }
}
