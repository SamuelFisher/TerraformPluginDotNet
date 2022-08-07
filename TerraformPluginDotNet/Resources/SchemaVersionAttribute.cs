namespace TerraformPluginDotNet.Resources;

/// <summary>
/// Indicates the version of a Terraform schema to allow version upgrades.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class SchemaVersionAttribute : Attribute
{
    public SchemaVersionAttribute(long schemaVersion)
    {
        SchemaVersion = schemaVersion;
    }

    public long SchemaVersion { get; }
}
