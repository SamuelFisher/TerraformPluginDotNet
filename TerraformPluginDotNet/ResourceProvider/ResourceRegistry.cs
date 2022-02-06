using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

public class ResourceRegistry
{
    public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

    public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

    public void RegisterResource<T>(string resourceName)
    {
        Schemas.Add(resourceName, SchemaBuilder.BuildSchema<T>());
        Types.Add(resourceName, typeof(T));
    }
}
