using TerraformPluginDotNet.Schemas;
using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

class ResourceRegistry
{
    public ResourceRegistry(
        ISchemaBuilder schemaBulder,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations)
    {
        foreach (var registration in resourceRegistrations)
        {
            Schemas.Add(registration.ResourceName, schemaBulder.BuildSchema(registration.Type));
            Types.Add(registration.ResourceName, registration.Type);
        }
        foreach (var registration in dataSourceRegistrations)
        {
            DataSchemas.Add(registration.ResourceName, schemaBulder.BuildSchema(registration.Type));
            Types.Add(registration.ResourceName, registration.Type);
        }
    }

    public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Schema> DataSchemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
}
