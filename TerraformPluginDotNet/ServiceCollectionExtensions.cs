using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet.ProviderConfig;
using TerraformPluginDotNet.Schemas;

namespace TerraformPluginDotNet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTerraformProviderConfigurator<TConfig, TProviderConfigurator>(this IServiceCollection services)
        where TProviderConfigurator : IProviderConfigurator<TConfig>
    {
        services.AddSingleton(s => new ProviderConfigurationRegistry
        {
            ConfigurationSchema = s.GetRequiredService<ISchemaBuilder>().BuildSchema(typeof(TConfig)),
            ConfigurationType = typeof(TConfig),
        });

        services.AddTransient<IProviderConfigurator<TConfig>>(s => s.GetRequiredService<TProviderConfigurator>());
        return services;
    }
}
