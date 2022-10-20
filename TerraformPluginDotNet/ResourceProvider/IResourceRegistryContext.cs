namespace TerraformPluginDotNet.ResourceProvider;

public interface IResourceRegistryContext
{
    void RegisterResource<T>(string resourceName);

    void RegisterDataSource<T>(string dataSourceName);
}
