using Google.Protobuf;
using MessagePack;
using MessagePack.Formatters;
using MessagePack.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Tfplugin5;

namespace TerraformPluginDotNet.Serialization
{
    public class DefaultDynamicValueSerializer : IDynamicValueSerializer
    {
        public T DeserializeJson<T>(ReadOnlyMemory<byte> value)
        {
            return JsonSerializer.Deserialize<T>(value.Span, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public T DeserializeMsgPack<T>(ReadOnlyMemory<byte> value)
        {
            return MessagePackSerializer.Deserialize<T>(value);
        }

        byte[] IDynamicValueSerializer.SerializeMsgPack<T>(T value)
        {
            return MessagePackSerializer.Serialize(value);
        }
    }
}
