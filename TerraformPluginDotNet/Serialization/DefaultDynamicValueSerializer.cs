using System.Text.Json;
using MessagePack;

namespace TerraformPluginDotNet.Serialization;

public class DefaultDynamicValueSerializer : IDynamicValueSerializer
{
    public T DeserializeJson<T>(ReadOnlyMemory<byte> value)
    {
        return JsonSerializer.Deserialize<T>(value.Span, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
             ?? throw new InvalidOperationException("Invalid Json provided");
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
