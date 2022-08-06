using System.Linq;
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
        var registry = new ResourceRegistry(new SchemaBuilder(), new[] { registration });

        var schema = registry.Schemas["test"];

        Assert.That(schema.Block, Is.Not.Null);

        var attributes = schema.Block.Attributes;
        Assert.That(attributes, Has.Count.EqualTo(6));

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
    }

    [TestCase("int_attribute", ExpectedResult = @"""number""")]
    [TestCase("boolean_attribute", ExpectedResult = @"""bool""")]
    [TestCase("float_attribute", ExpectedResult = @"""number""")]
    [TestCase("double_attribute", ExpectedResult = @"""number""")]
    public string TestSchemaAttributeTypes(string name)
    {
        var registration = new ResourceRegistryRegistration("test", typeof(TestResource));
        var registry = new ResourceRegistry(new SchemaBuilder(), new[] { registration });

        var schema = registry.Schemas["test"];

        Assert.That(schema.Block, Is.Not.Null, $"Unable to find attribute '{name}' on schema.");

        var attributes = schema.Block.Attributes;
        var attr = attributes.SingleOrDefault(x => x.Name == name);
        Assert.That(attr, Is.Not.Null);
        return attr.Type.ToStringUtf8();
    }
}
