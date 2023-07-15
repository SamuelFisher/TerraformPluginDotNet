using System.Collections.Immutable;
using System.Reflection;
using TerraformPluginDotNet.Resources;
using KeyAttribute = MessagePack.KeyAttribute;

namespace TerraformPluginDotNet.Schemas.Types;

class TerraformTypeBuilder : ITerraformTypeBuilder
{
    public TerraformType GetTerraformType(Type t)
    {
        if (t.IsValueType && Nullable.GetUnderlyingType(t) is Type underlyingType)
        {
            t = underlyingType;
        }

        if (t == typeof(string))
        {
            return new TerraformType.TfString();
        }

        if (t == typeof(int) || t == typeof(float) || t == typeof(double))
        {
            return new TerraformType.TfNumber();
        }

        if (t == typeof(bool))
        {
            return new TerraformType.TfBool();
        }

        var dictionaryType = t.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>) && x.GenericTypeArguments[0] == typeof(string));

        if (dictionaryType != null)
        {
            var valueType = GetTerraformType(t.GenericTypeArguments[1]);
            return new TerraformType.TfMap(valueType);
        }

        var collectionType = t.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

        if (collectionType != null)
        {
            var elementType = GetTerraformType(collectionType.GenericTypeArguments.Single());
            return new TerraformType.TfList(elementType);
        }

        return GetTerraformTypeAsObject(t);
    }

    private TerraformType GetTerraformTypeAsObject(Type t)
    {
        var properties = t.GetProperties();
        var attrTypes = properties.ToDictionary(
            prop => prop.GetCustomAttribute<KeyAttribute>()?.StringKey ?? throw new InvalidOperationException($"Missing {nameof(KeyAttribute)} on {prop.Name} in {t.Name}."),
            prop => GetTerraformType(prop.PropertyType));
        var optionalAttrs = properties.Where(x => !IsRequiredAttribute(x)).Select(x => x.GetCustomAttribute<KeyAttribute>()?.StringKey ?? throw new InvalidOperationException($"Missing {nameof(KeyAttribute)} on {x.Name} in {t.Name}.")).ToList();
        return new TerraformType.TfObject(attrTypes.ToImmutableDictionary(), optionalAttrs.ToImmutableHashSet());
    }

    public static bool IsRequiredAttribute(PropertyInfo property)
    {
        return property.GetCustomAttribute<RequiredAttribute>() != null ||
            (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null);
    }
}
