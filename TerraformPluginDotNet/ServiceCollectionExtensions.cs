using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet.ProviderConfig;

namespace TerraformPluginDotNet;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTerraformProviderConfigurator<TConfig, TProviderConfigurator>(this IServiceCollection services)
        where TProviderConfigurator : IProviderConfigurator<TConfig>
    {
        var providerConfigRegistry = new ProviderConfigurationRegistry
        {
            ConfigurationSchema = SchemaBuilder.BuildSchema<TConfig>(),
            ConfigurationType = typeof(TConfig),
        };

        services.AddSingleton(providerConfigRegistry);
        services.AddTransient<IProviderConfigurator<TConfig>>(s => s.GetRequiredService<TProviderConfigurator>());
        return services;
    }
}
