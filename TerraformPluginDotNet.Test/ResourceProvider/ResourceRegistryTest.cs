using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas;

namespace TerraformPluginDotNet.Test.ResourceProvider;

[TestFixture]
public class ResourceRegistryTest
{
    [Test]
    public void TestRegisteredSchema()
    {
        var registration = new ResourceRegistryRegistration("test", typeof(TestResource));
        var registry = new ResourceRegistry(new SchemaBuilder(NullLogger<SchemaBuilder>.Instance), new[] { registration });

        var schema = registry.Schemas["test"];

        Assert.That(schema.Block, Is.Not.Null);

        var attributes = schema.Block.Attributes;
        Assert.That(attributes, Has.Count.EqualTo(11));

        var idAttr = attributes.Single(x => x.Name == "id");
        Assert.That(idAttr.Type.ToStringUtf8(), Is.EqualTo("\"string\""));
        Assert.That(idAttr.Description, Is.EqualTo("Unique ID for this resource."));
        Assert.That(idAttr.Optional, Is.True);
        Assert.That(idAttr.Required, Is.False);
        Assert.That(idAttr.Computed, Is.True);

        var requiredStrAttr = attributes.Single(x => x.Name == "required_string");
        Assert.That(requiredStrAttr.Type.ToStringUtf8(), Is.EqualTo("\"string\""));
        Assert.That(requiredStrAttr.Description, Is.EqualTo("This is required."));
        Assert.That(requiredStrAttr.Optional, Is.False);
        Assert.That(requiredStrAttr.Required, Is.True);
        Assert.That(requiredStrAttr.Computed, Is.False);

        var requiredIntAttr = attributes.Single(x => x.Name == "required_int");
        Assert.That(requiredIntAttr.Type.ToStringUtf8(), Is.EqualTo("\"number\""));
        Assert.That(requiredIntAttr.Description, Is.EqualTo("This is required."));
        Assert.That(requiredIntAttr.Optional, Is.False);
        Assert.That(requiredIntAttr.Required, Is.True);
        Assert.That(requiredIntAttr.Computed, Is.False);
    }

    [TestCase("int_attribute", ExpectedResult = @"""number""")]
    [TestCase("boolean_attribute", ExpectedResult = @"""bool""")]
    [TestCase("float_attribute", ExpectedResult = @"""number""")]
    [TestCase("double_attribute", ExpectedResult = @"""number""")]
    [TestCase("string_list_attribute", ExpectedResult = @"[""list"",""string""]")]
    [TestCase("int_list_attribute", ExpectedResult = @"[""list"",""number""]")]
    [TestCase("string_map_attribute", ExpectedResult = @"[""map"",""string""]")]
    [TestCase("int_map_attribute", ExpectedResult = @"[""map"",""number""]")]
    public string TestSchemaAttributeTypes(string name)
    {
        var registration = new ResourceRegistryRegistration("test", typeof(TestResource));
        var registry = new ResourceRegistry(new SchemaBuilder(NullLogger<SchemaBuilder>.Instance), new[] { registration });

        var schema = registry.Schemas["test"];

        Assert.That(schema.Block, Is.Not.Null, $"Unable to find attribute '{name}' on schema.");

        var attributes = schema.Block.Attributes;
        var attr = attributes.SingleOrDefault(x => x.Name == name);
        Assert.That(attr, Is.Not.Null);
        return attr.Type.ToStringUtf8();
    }
}
