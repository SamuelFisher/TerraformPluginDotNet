using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MessagePack;
using NUnit.Framework;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Resources;
using Description = System.ComponentModel.DescriptionAttribute;
using Key = MessagePack.KeyAttribute;

namespace TerraformPluginDotNet.Test.ResourceProvider;

[TestFixture]
public class ResourceRegistryTest
{
    [Test]
    public void TestRegisteredSchema()
    {
        var registry = new ResourceRegistry();
        registry.RegisterResource<TestResource>("test");

        var schema = registry.Schemas["test"];

        Assert.That(schema.Block, Is.Not.Null);

        var attributes = schema.Block.Attributes;
        Assert.That(attributes, Has.Count.EqualTo(3));

        var idAttr = attributes.Single(x => x.Name == "id");
        Assert.That(idAttr.Type.ToStringUtf8(), Is.EqualTo("\"string\""));
        Assert.That(idAttr.Description, Is.EqualTo("Unique ID for this resource."));
        Assert.That(idAttr.Optional, Is.True);
        Assert.That(idAttr.Required, Is.False);
        Assert.That(idAttr.Computed, Is.True);

        var requiredAttr = attributes.Single(x => x.Name == "required_attribute");
        Assert.That(requiredAttr.Type.ToStringUtf8(), Is.EqualTo("\"string\""));
        Assert.That(requiredAttr.Description, Is.EqualTo("This is required."));
        Assert.That(requiredAttr.Optional, Is.False);
        Assert.That(requiredAttr.Required, Is.True);
        Assert.That(requiredAttr.Computed, Is.False);

        var optionalAttr = attributes.Single(x => x.Name == "optional_attribute");
        Assert.That(optionalAttr.Type.ToStringUtf8(), Is.EqualTo("\"number\""));
        Assert.That(optionalAttr.Description, Is.EqualTo("This is optional."));
        Assert.That(optionalAttr.Optional, Is.True);
        Assert.That(optionalAttr.Required, Is.False);
        Assert.That(optionalAttr.Computed, Is.False);
    }

    [MessagePackObject]
    private class TestResource
    {
        [Key("id")]
        [Computed]
        [Description("Unique ID for this resource.")]
        public string Id { get; set; }

        [Key("required_attribute")]
        [Description("This is required.")]
        [Required]
        public string RequiredAttribute { get; set; }

        [Key("optional_attribute")]
        [Description("This is optional.")]
        public int OptionalAttribute { get; set; }
    }
}
