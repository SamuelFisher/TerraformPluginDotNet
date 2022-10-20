namespace TerraformPluginDotNet.ResourceProvider;

public interface IDataSourceProvider<T>
{
    Task<T> ReadAsync(T request);
}
