namespace TerraformPluginDotNet.Resources;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SchemaVersionAttribute : Attribute
{
    public SchemaVersionAttribute(long schemaVersion)
    {
        SchemaVersion = schemaVersion;
    }

    public long SchemaVersion { get; }
}
