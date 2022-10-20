using TerraformPluginDotNet.Serialization;

namespace TerraformPluginDotNet.ResourceProvider;

/// <summary>
/// Default implementation of a resource upgrader that does nothing, and assumes
/// previous versions can be deserialized from JSON into the latest schema version.
/// </summary>
class DefaultResourceUpgrader<T> : IResourceUpgrader<T>
{
    private readonly IDynamicValueSerializer _serializer;

    public DefaultResourceUpgrader(IDynamicValueSerializer serializer)
    {
        _serializer = serializer;
    }

    public Task<T> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json)
    {
        return Task.FromResult(_serializer.DeserializeJson<T>(json) ?? throw new InvalidOperationException("Invalid Json provided"));
    }
}
