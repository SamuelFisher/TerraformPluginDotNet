using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using TerraformPluginDotNet.Resources;
using Tfplugin5;

namespace TerraformPluginDotNet.Schemas;

class SchemaBuilder : ISchemaBuilder
{
    private readonly ILogger<SchemaBuilder> _logger;

    public SchemaBuilder(ILogger<SchemaBuilder> logger)
    {
        _logger = logger;
    }

    public Schema BuildSchema(Type type)
    {
        var schemaVersionAttribute = type.GetCustomAttribute<SchemaVersionAttribute>();
        if (schemaVersionAttribute == null)
        {
            _logger.LogWarning($"Missing {nameof(SchemaVersionAttribute)} when generating schema for {type.FullName}.");
        }

        var properties = type.GetProperties();

        var block = new Schema.Types.Block();
        foreach (var property in properties)
        {
            var key = property.GetCustomAttribute<MessagePack.KeyAttribute>();
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var required = IsRequiredAttribute(property);
            var computed = property.GetCustomAttribute<ComputedAttribute>() != null;

            block.Attributes.Add(new Schema.Types.Attribute
            {
                Name = key.StringKey,
                Type = ByteString.CopyFromUtf8(GetTerraformType(property.PropertyType)),
                Description = description?.Description,
                Optional = !required,
                Required = required,
                Computed = computed,
            });
        }

        return new Schema
        {
            Version = schemaVersionAttribute?.SchemaVersion ?? 0,
            Block = block,
        };
    }

    private static bool IsRequiredAttribute(PropertyInfo property)
    {
        return property.GetCustomAttribute<RequiredAttribute>() != null ||
            (property.PropertyType.IsValueType && Nullable.GetUnderlyingType(property.PropertyType) == null);
    }

    private static string GetTerraformType(Type t)
    {
        if (t.IsValueType && Nullable.GetUnderlyingType(t) != null)
        {
            t = Nullable.GetUnderlyingType(t);
        }

        if (t == typeof(string))
        {
            return "\"string\"";
        }

        if (t == typeof(int) || t == typeof(float) || t == typeof(double))
        {
            return "\"number\"";
        }

        if (t == typeof(bool))
        {
            return "\"bool\"";
        }

        var dictionaryType = t.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IDictionary<,>) && x.GenericTypeArguments[0] == typeof(string));

        if (dictionaryType != null)
        {
            var valueType = GetTerraformType(t.GenericTypeArguments[1]);
            return $"[\"map\",{valueType}]";
        }

        var collectionType = t.GetInterfaces()
            .FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

        if (collectionType != null)
        {
            var elementType = GetTerraformType(collectionType.GenericTypeArguments.Single());
            return $"[\"list\",{elementType}]";
        }

        throw new NotSupportedException($"Unable to convert {t.FullName} to Terraform type.");
    }
}
