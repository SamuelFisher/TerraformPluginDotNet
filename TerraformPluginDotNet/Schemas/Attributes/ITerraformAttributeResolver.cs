using System.Reflection;

namespace TerraformPluginDotNet.Schemas.Attributes;

public interface ITerraformAttributeResolver
{
    string GetKey(PropertyInfo property);

    string? GetDescription(PropertyInfo property);

    bool IsRequired(PropertyInfo property);

    bool IsComputed(PropertyInfo property);

    bool? IsOptional(PropertyInfo property);
}
