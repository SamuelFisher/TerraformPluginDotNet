namespace TerraformPluginDotNet.ResourceProvider;

public interface IResourceUpgrader<T>
{
    Task<T> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json);
}
