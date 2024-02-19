using System.ComponentModel;
using System.Reflection;
using MessagePack;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Schemas.Types;

namespace TerraformPluginDotNet.Schemas.Attributes;

public class TerraformAttributeResolver : ITerraformAttributeResolver
{
    public string? GetDescription(PropertyInfo property)
    {
        return property.GetCustomAttribute<DescriptionAttribute>()?.Description;
    }

    public string GetKey(PropertyInfo property)
    {
        return property.GetCustomAttribute<KeyAttribute>()?.StringKey ?? throw new InvalidOperationException($"Missing {nameof(KeyAttribute)} on {property.Name} in {property.DeclaringType?.Name}.");
    }

    public bool IsComputed(PropertyInfo property)
    {
        return property.GetCustomAttribute<ComputedAttribute>() is not null;
    }

    public bool? IsOptional(PropertyInfo property)
    {
        return null;
    }

    public bool IsRequired(PropertyInfo property)
    {
        return TerraformTypeBuilder.IsRequiredAttribute(property);
    }
}
