namespace TerraformPluginDotNet.ProviderConfig;

public interface IProviderConfigurator<T>
{
    Task ConfigureAsync(T config);
}
