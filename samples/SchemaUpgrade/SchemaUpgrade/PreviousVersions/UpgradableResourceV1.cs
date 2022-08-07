using System.Text.Json.Serialization;
using TerraformPluginDotNet.Resources;

namespace SchemaUpgrade.PreviousVersions;

/// <summary>
/// An example resource type that can be upgraded. This is V1.
/// </summary>
[SchemaVersion(1)]
internal class UpgradableResourceV1
{
    public string Id { get; set; }

    // Renamed to `Data` in V2.
    public string Value { get; set; }
}
