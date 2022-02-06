using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Google.Protobuf;
using TerraformPluginDotNet.Resources;
using Tfplugin5;

namespace TerraformPluginDotNet;

static class SchemaBuilder
{
    public static Schema BuildSchema<T>()
    {
        var properties = typeof(T).GetProperties();

        var block = new Schema.Types.Block();
        foreach (var property in properties)
        {
            var key = property.GetCustomAttribute<MessagePack.KeyAttribute>();
            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var required = property.GetCustomAttribute<RequiredAttribute>() != null;
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

    private static string GetTerraformType(Type t)
    {
        if (t == typeof(string))
        {
            return "string";
        }

        if (t == typeof(int))
        {
            return "number";
        }

        throw new NotSupportedException();
    }
}
