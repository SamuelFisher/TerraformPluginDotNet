using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;
using Key = MessagePack.KeyAttribute;

namespace SampleProvider;

[SchemaVersion(1)]
[MessagePackObject]
public class SampleFileResource
{
    [Key("id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string Id { get; set; }

    [Key("path")]
    [Description("Path to the file.")]
    [Required]
    public string Path { get; set; }

    [Key("content")]
    [Description("Contents of the file.")]
    [Required]
    public string Content { get; set; }
}
