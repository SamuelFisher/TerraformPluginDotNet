using System.Collections.Immutable;
using System.Reflection;
using TerraformPluginDotNet.Resources;
using KeyAttribute = MessagePack.KeyAttribute;
using MessagePackObject = MessagePack.MessagePackObjectAttribute;

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

        var genericType = t.IsGenericType ? t.GetGenericTypeDefinition() : null;

        if (genericType == typeof(Tuple<>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type));
        }

        if (genericType == typeof(Tuple<,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type));
        }

        if (genericType == typeof(Tuple<,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type));
        }

        if (genericType == typeof(Tuple<,,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            var el3Type = GetTerraformType(t.GenericTypeArguments[3]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type, el3Type));
        }

        if (genericType == typeof(Tuple<,,,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            var el3Type = GetTerraformType(t.GenericTypeArguments[3]);
            var el4Type = GetTerraformType(t.GenericTypeArguments[4]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type, el3Type, el4Type));
        }

        if (genericType == typeof(Tuple<,,,,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            var el3Type = GetTerraformType(t.GenericTypeArguments[3]);
            var el4Type = GetTerraformType(t.GenericTypeArguments[4]);
            var el5Type = GetTerraformType(t.GenericTypeArguments[5]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type, el3Type, el4Type, el5Type));
        }

        if (genericType == typeof(Tuple<,,,,,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            var el3Type = GetTerraformType(t.GenericTypeArguments[3]);
            var el4Type = GetTerraformType(t.GenericTypeArguments[4]);
            var el5Type = GetTerraformType(t.GenericTypeArguments[5]);
            var el6Type = GetTerraformType(t.GenericTypeArguments[6]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type, el3Type, el4Type, el5Type, el6Type));
        }

        if (genericType == typeof(Tuple<,,,,,,,>))
        {
            var el0Type = GetTerraformType(t.GenericTypeArguments[0]);
            var el1Type = GetTerraformType(t.GenericTypeArguments[1]);
            var el2Type = GetTerraformType(t.GenericTypeArguments[2]);
            var el3Type = GetTerraformType(t.GenericTypeArguments[3]);
            var el4Type = GetTerraformType(t.GenericTypeArguments[4]);
            var el5Type = GetTerraformType(t.GenericTypeArguments[5]);
            var el6Type = GetTerraformType(t.GenericTypeArguments[6]);
            var el7Type = GetTerraformType(t.GenericTypeArguments[7]);
            return new TerraformType.TfTuple(ImmutableList.Create(el0Type, el1Type, el2Type, el3Type, el4Type, el5Type, el6Type, el7Type));
        }

        var genericInterfaces = t.GetInterfaces()
            .Where(x => x.IsGenericType)
            .GroupBy(x => x.GetGenericTypeDefinition())
            .ToDictionary(x => x.Key, x => x.First());

        if (genericInterfaces.TryGetValue(typeof(IDictionary<,>), out var dictType))
        {
            var valueType = GetTerraformType(dictType.GenericTypeArguments[1]);
            return new TerraformType.TfMap(valueType);
        }

        if (genericInterfaces.TryGetValue(typeof(ISet<>), out var setType))
        {
            var elementType = GetTerraformType(setType.GenericTypeArguments.Single());
            return new TerraformType.TfSet(elementType);
        }

        if (genericInterfaces.TryGetValue(typeof(ICollection<>), out var collectionType))
        {
            var elementType = GetTerraformType(collectionType.GenericTypeArguments.Single());
            return new TerraformType.TfList(elementType);
        }

        return GetTerraformTypeAsObject(t);
    }

    private TerraformType GetTerraformTypeAsObject(Type t)
    {
        if (t.GetCustomAttribute<MessagePackObject>() == null)
        {
            throw new InvalidOperationException($"Type {t.Name} is represented as a Terraform object, but is missing a {nameof(MessagePackObject)} attribute.");
        }

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
