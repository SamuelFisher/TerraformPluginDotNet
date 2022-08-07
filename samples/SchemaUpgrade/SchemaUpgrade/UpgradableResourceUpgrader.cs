using System;
using System.Threading.Tasks;
using SchemaUpgrade.PreviousVersions;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Serialization;

namespace SchemaUpgrade;

/// <summary>
/// Updates <see cref="PreviousVersions.UpgradableResourceV1"/> to <see cref="UpgradableResourceV2"/>.
/// </summary>
public class UpgradableResourceUpgrader : IResourceUpgrader<UpgradableResourceV2>
{
    private readonly IDynamicValueSerializer _serializer;

    public UpgradableResourceUpgrader(IDynamicValueSerializer serializer)
    {
        _serializer = serializer;
    }

    public Task<UpgradableResourceV2> UpgradeResourceStateAsync(long schemaVersion, ReadOnlyMemory<byte> json)
    {
        switch (schemaVersion)
        {
            case 1:
                // Upgrade from V1 to V2.
                var v1 = _serializer.DeserializeJson<UpgradableResourceV1>(json);
                var v2 = new UpgradableResourceV2
                {
                    Id = v1.Id,
                    Data = v1.Value,
                };
                return Task.FromResult(v2);
            case 2:
                // Already the latest version.
                return Task.FromResult(_serializer.DeserializeJson<UpgradableResourceV2>(json));
            default:
                throw new NotSupportedException();
        }
    }
}
