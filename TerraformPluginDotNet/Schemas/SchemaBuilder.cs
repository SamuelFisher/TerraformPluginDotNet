using System.ComponentModel;
using System.Reflection;
using Google.Protobuf;
using Microsoft.Extensions.Logging;
using TerraformPluginDotNet.Resources;
using TerraformPluginDotNet.Schemas.Attributes;
using TerraformPluginDotNet.Schemas.Types;
using Tfplugin5;
using KeyAttribute = MessagePack.KeyAttribute;

namespace TerraformPluginDotNet.Schemas;

public class SchemaBuilder : ISchemaBuilder
{
    private readonly ILogger<SchemaBuilder> _logger;
    private readonly ITerraformTypeBuilder _typeBuilder;
    private readonly ITerraformAttributeResolver _attributeResolver;

    public SchemaBuilder(ILogger<SchemaBuilder> logger, ITerraformTypeBuilder typeBuilder, ITerraformAttributeResolver attributeResolver)
    {
        _logger = logger;
        _typeBuilder = typeBuilder;
        _attributeResolver = attributeResolver;
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
            var key = _attributeResolver.GetKey(property);

            var description = _attributeResolver.GetDescription(property);
            var required = _attributeResolver.IsRequired(property);
            var computed = _attributeResolver.IsComputed(property);
            var optional = _attributeResolver.IsOptional(property) ?? !required;
            var terraformType = _typeBuilder.GetTerraformType(property.PropertyType);

            if (terraformType is TerraformType.TfObject _ && !required)
            {
                throw new InvalidOperationException("Optional object types are not supported.");
            }

            block.Attributes.Add(new Schema.Types.Attribute
            {
                Name = key,
                Type = ByteString.CopyFromUtf8(terraformType.ToJson()),
                Description = description,
                Required = required,
                Computed = computed,
                Optional = optional,
            });
        }

        return new Schema
        {
            Version = schemaVersionAttribute?.SchemaVersion ?? 0,
            Block = block,
        };
    }
}
