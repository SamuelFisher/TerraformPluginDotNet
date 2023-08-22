using TerraformPluginDotNet.Schemas;
using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

class ResourceRegistry
{
    public ResourceRegistry(
        ISchemaBuilder schemaBuilder,
        IEnumerable<ResourceRegistryRegistration> resourceRegistrations,
        IEnumerable<DataSourceRegistryRegistration> dataSourceRegistrations,
        IEnumerable<IDataSourceSchemaProvider> dataSourceSchemaProviders)
    {
        foreach (var registration in resourceRegistrations)
        {
            Schemas.Add(registration.ResourceName, schemaBuilder.BuildSchema(registration.Type));
            Types.Add(registration.ResourceName, registration.Type);
        }
        foreach (var registration in dataSourceRegistrations)
        {
            DataSchemas.Add(registration.ResourceName, schemaBuilder.BuildSchema(registration.Type));
            DataTypes.Add(registration.ResourceName, registration.Type);
        }
        foreach (var provider in dataSourceSchemaProviders)
        {
            foreach (var registration in provider.GetSchemas())
            {
                DataSchemas.Add(registration.ResourceName, registration.Schema);
                DataTypes.Add(registration.ResourceName, registration.Type);
            }
        }
    }

    public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Schema> DataSchemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

    public Dictionary<string, Type> DataTypes { get; } = new Dictionary<string, Type>();
}
