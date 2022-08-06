using TerraformPluginDotNet.Schemas;
using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

class ResourceRegistry
{
    public ResourceRegistry(ISchemaBuilder schemaBulder, IEnumerable<ResourceRegistryRegistration> registrations)
    {
        foreach (var registration in registrations)
        {
            Schemas.Add(registration.ResourceName, schemaBulder.BuildSchema(registration.Type));
            Types.Add(registration.ResourceName, registration.Type);
        }
    }

    public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();
}
