using System.Collections.Generic;
using System.ComponentModel;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Serialization;

namespace TerraformPluginDotNet.Test;

[SchemaVersion(1)]
[MessagePackObject]
public class TestResource
{
    [Key("id")]
    [Computed]
    [Description("Unique ID for this resource.")]
    [MessagePackFormatter(typeof(ComputedStringValueFormatter))]
    public string Id { get; set; }

    [Key("required_string")]
    [Description("This is required.")]
    [Required]
    public string RequiredString { get; set; }

    [Key("required_int")]
    [Description("This is required.")]
    public int RequiredInt { get; set; }

    [Key("int_attribute")]
    [Description("Int attribute.")]
    public int? IntAttribute { get; set; }

    [Key("boolean_attribute")]
    [Description("A boolean attribute.")]
    public bool? BooleanAttribute { get; set; }

    [Key("float_attribute")]
    [Description("A float attribute.")]
    public float? FloatAttribute { get; set; }

    [Key("double_attribute")]
    [Description("A double attribute.")]
    public float? DoubleAttribute { get; set; }

    [Key("string_list_attribute")]
    [Description("A string list attribute.")]
    public List<string> StringListAttribute { get; set; }

    [Key("int_list_attribute")]
    [Description("An int list attribute.")]
    public List<int> IntListAttribute { get; set; }

    [Key("string_map_attribute")]
    [Description("A string map attribute.")]
    public Dictionary<string, string> StringMapAttribute { get; set; }

    [Key("int_map_attribute")]
    [Description("An int map attribute.")]
    public Dictionary<string, int> IntMapAttribute { get; set; }

    [Key("object_1")]
    [Description("An object.")]
    [Required]
    public TestObjectWithOptionalAttributes Object1 { get; set; }

    [Key("object_2")]
    [Description("An object.")]
    [Required]
    public TestObjectNoOptionalAttributes Object2 { get; set; }
}

[MessagePackObject]
public class TestObjectWithOptionalAttributes
{
    [Key("required_string")]
    [Required]
    public string RequiredString { get; set; }

    [Key("nested_int")]
    public int? NestedInt { get; set; }
}

[MessagePackObject]
public class TestObjectNoOptionalAttributes
{
    [Key("required_string")]
    [Required]
    public string RequiredString { get; set; }
}
