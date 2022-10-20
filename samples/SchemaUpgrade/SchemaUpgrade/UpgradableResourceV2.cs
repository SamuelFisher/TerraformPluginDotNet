using System.ComponentModel;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace SchemaUpgrade;

/// <summary>
/// An example resource type that can be upgraded. This is V2.
/// </summary>
[SchemaVersion(2)]
[MessagePackObject]
public class UpgradableResourceV2
{
    [Key("id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Id { get; set; }

    // Renamed from `Value` in V1.
    [Key("data")]
    [Description("Some data.")]
    [Required]
    public string Data { get; set; } = null!;
}
