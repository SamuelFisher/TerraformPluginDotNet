namespace TerraformPluginDotNet.Serialization;

public interface IDynamicValueSerializer
{
    T DeserializeJson<T>(ReadOnlyMemory<byte> value);

    T DeserializeMsgPack<T>(ReadOnlyMemory<byte> value);

    byte[] SerializeMsgPack<T>(T value);
}
