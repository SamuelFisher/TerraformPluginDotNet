namespace TerraformPluginDotNet.ResourceProvider;

public interface IResourceProvider<T>
{
    Task<T> PlanAsync(T prior, T proposed);

    Task<T> CreateAsync(T planned);

    Task<T> ReadAsync(T resource);

    Task<T> UpdateAsync(T prior, T planned);

    Task DeleteAsync(T resource);
}
