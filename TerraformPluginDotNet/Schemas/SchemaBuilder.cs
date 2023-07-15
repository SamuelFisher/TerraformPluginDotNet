using System.ComponentModel;
using System.Reflection;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;
using KeyAttribute = MessagePack.KeyAttribute;

namespace TerraformPluginDotNet.Schemas;

class SchemaBuilder : ISchemaBuilder
{
    private readonly ILogger<SchemaBuilder> _logger;
    private readonly ITerraformTypeBuilder _typeBuilder;

    public SchemaBuilder(ILogger<SchemaBuilder> logger, ITerraformTypeBuilder typeBuilder)
    {
        _logger = logger;
        _typeBuilder = typeBuilder;
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
            var key = property.GetCustomAttribute<KeyAttribute>() ?? throw new InvalidOperationException($"Missing {nameof(KeyAttribute)} on {property.Name} in {type.Name}.");

            var description = property.GetCustomAttribute<DescriptionAttribute>();
            var required = TerraformTypeBuilder.IsRequiredAttribute(property);
            var computed = property.GetCustomAttribute<ComputedAttribute>() != null;
            var terraformType = _typeBuilder.GetTerraformType(property.PropertyType);

            if (terraformType is TerraformType.TfObject _ && !required)
            {
                throw new InvalidOperationException("Optional object types are not supported.");
            }

            block.Attributes.Add(new Schema.Types.Attribute
            {
                Name = key.StringKey,
                Type = ByteString.CopyFromUtf8(terraformType.ToJson()),
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
}
