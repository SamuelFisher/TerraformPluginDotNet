using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Google.Protobuf;
using TerraformPluginDotNet.Resources;
using Tfplugin5;

namespace TerraformPluginDotNet.Schemas;

class SchemaBuilder : ISchemaBuilder
{
    public Schema BuildSchema(Type type)
    {
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
                Type = ByteString.CopyFromUtf8($"\"{GetTerraformType(property.PropertyType)}\""),
                Description = description.Description,
                Optional = !required,
                Required = required,
                Computed = computed,
            });
        }

        return new Schema
        {
            Version = 0,
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
            return "string";
        }

        if (t == typeof(int) || t == typeof(float) || t == typeof(double))
        {
            return "number";
        }

        if (t == typeof(bool))
        {
            return "bool";
        }

        throw new NotSupportedException();
    }
}
