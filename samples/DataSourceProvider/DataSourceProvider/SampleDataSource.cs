using System.ComponentModel;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace DataSourceProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class SampleDataSource
{
    [Key("id")]
    [Description("Id")]
    [Required]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Id { get; set; }

    [Key("data")]
    [Description("Dummy data.")]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string? Data { get; set; }
}
