namespace TerraformPluginDotNet.ResourceProvider;

/// <summary>
/// Upgrades resource state from previous schema versions to the latest version.
/// </summary>
/// <typeparam name="T">The type representing the latest schema of the resource.</typeparam>
public interface IResourceUpgrader<T>
{
    /// <summary>
    /// Upgrades a resources state from an older schema to the latest schema.
    /// </summary>
    /// <param name="schemaVersion">Schema version to upgrade from.</param>
    /// <param name="json">Raw data to be upgraded.</param>
    /// <returns>The resource state represented in the latest schema.</returns>
    Task<T> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json);
}
