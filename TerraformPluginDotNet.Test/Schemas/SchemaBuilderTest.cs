using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using TerraformPluginDotNet.Schemas;
using TerraformPluginDotNet.Schemas.Attributes;
using TerraformPluginDotNet.Schemas.Types;

namespace TerraformPluginDotNet.Test.Schemas;

[TestFixture]
public class SchemaBuilderTest
{
    private SchemaBuilder _schemaBuilder;

    [SetUp]
    public void Setup()
    {
        _schemaBuilder = new SchemaBuilder(
            NullLogger<SchemaBuilder>.Instance,
            new TerraformTypeBuilder(),
            new TerraformAttributeResolver());
    }

    [Test]
    public void TestSchema()
    {
        var schema = _schemaBuilder.BuildSchema(typeof(TestResource));

        Assert.That(schema.Block, Is.Not.Null);

        var attributes = schema.Block.Attributes;
        Assert.That(attributes, Has.Count.EqualTo(15));

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

        var requiredObjectAttr = attributes.Single(x => x.Name == "object_1");
        Assert.That(
            requiredObjectAttr.Type.ToStringUtf8(),
            Is.EqualTo("""["object",{"nested_int":"number","required_string":"string"},["nested_int"]]"""));
        Assert.That(requiredObjectAttr.Description, Is.EqualTo("An object."));
        Assert.That(requiredObjectAttr.Optional, Is.False);
        Assert.That(requiredObjectAttr.Required, Is.True);
        Assert.That(requiredObjectAttr.Computed, Is.False);
    }
}
