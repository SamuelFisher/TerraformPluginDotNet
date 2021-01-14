using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TerraformPluginDotNet.Resources;
using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider
{
    public class ResourceRegistry
    {
        public Dictionary<string, Schema> Schemas { get; } = new Dictionary<string, Schema>();

        public Dictionary<string, Type> Types { get; } = new Dictionary<string, Type>();

        public void RegisterResource<T>(string resourceName)
        {
            Schemas.Add(resourceName, BuildSchema<T>());
            Types.Add(resourceName, typeof(T));
        }

        private Schema BuildSchema<T>()
        {
            var properties = typeof(T).GetProperties();

            var block = new Schema.Types.Block();
            foreach(var property in properties)
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
}
