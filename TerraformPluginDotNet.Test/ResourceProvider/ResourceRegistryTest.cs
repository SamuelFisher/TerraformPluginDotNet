using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using TerraformPluginDotNet.ResourceProvider;
using TerraformPluginDotNet.Schemas;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;

namespace TerraformPluginDotNet.Test.ResourceProvider;

[TestFixture]
public class ResourceRegistryTest
{
    [Test]
    public void TestRegisterResource()
    {
        var registry = BuildRegistry();

        Assert.That(registry.Schemas.Keys, Is.EquivalentTo(new[] { "test_resource" }));
        Assert.That(registry.Schemas["test_resource"], Is.InstanceOf<Schema>());

        Assert.That(registry.Types.Keys, Is.EquivalentTo(new[] { "test_resource" }));
        Assert.That(registry.Types["test_resource"], Is.EqualTo(typeof(TestResource)));
    }

    [Test]
    public void TestRegisterDataSource()
    {
        var registry = BuildRegistry();

        Assert.That(registry.DataSchemas.Keys, Is.EquivalentTo(new[] { "test_data_source" }));
        Assert.That(registry.DataSchemas["test_data_source"], Is.InstanceOf<Schema>());

        Assert.That(registry.DataTypes.Keys, Is.EquivalentTo(new[] { "test_data_source" }));
        Assert.That(registry.DataTypes["test_data_source"], Is.EqualTo(typeof(TestResource)));
    }

    private static ResourceRegistry BuildRegistry()
    {
        var registration = new ResourceRegistryRegistration("test_resource", typeof(TestResource));
        var dataSourceRegistration = new DataSourceRegistryRegistration("test_data_source", typeof(TestResource));
        var registry = new ResourceRegistry(new SchemaBuilder(NullLogger<SchemaBuilder>.Instance, new TerraformTypeBuilder()), new[] { registration }, new[] { dataSourceRegistration });
        return registry;
    }
}
