using Microsoft.Extensions.DependencyInjection;
using TerraformPluginDotNet.ProviderConfig;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas;
using TerraformPluginDotNet.Schemas.Types;
using TerraformPluginDotNet.Serialization;

namespace TerraformPluginDotNet;

/// <summary>
/// Use the extensions in this class for more granular configuration of the Terraform plugin.
/// For simpler setup, use <see cref="TerraformPluginHost"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddTerraformPluginCore(this IServiceCollection services)
    {
        services.AddTransient<ITerraformTypeBuilder, TerraformTypeBuilder>();
        services.AddTransient<ISchemaBuilder, SchemaBuilder>();
        services.AddTransient(typeof(ProviderConfigurationHost<>));
        services.AddTransient(typeof(ResourceProviderHost<>));
        services.AddTransient(typeof(DataSourceProviderHost<>));
        services.AddTransient(typeof(IResourceUpgrader<>), typeof(DefaultResourceUpgrader<>));
        services.AddTransient<IDynamicValueSerializer, DefaultDynamicValueSerializer>();
        return services;
    }

    public static IResourceRegistryContext AddTerraformResourceRegistry(this IServiceCollection services)
    {
        services.AddOptions<TerraformPluginHostOptions>().ValidateDataAnnotations();
        services.AddSingleton<ResourceRegistry>();

        var registryContext = new ServiceCollectionResourceRegistryContext(services);
        return registryContext;
    }

    /// <summary>
    /// Adds a configurator that will be called when configuring this terraform plugin.
    /// </summary>
    public static IServiceCollection AddTerraformProviderConfigurator<TConfig, TProviderConfigurator>(this IServiceCollection services)
        where TProviderConfigurator : IProviderConfigurator<TConfig>
    {
        services.AddSingleton(s => new ProviderConfigurationRegistry(
            ConfigurationSchema: s.GetRequiredService<ISchemaBuilder>().BuildSchema(typeof(TConfig)),
            ConfigurationType: typeof(TConfig)));

        services.AddTransient<IProviderConfigurator<TConfig>>(s => s.GetRequiredService<TProviderConfigurator>());
        return services;
    }
}
