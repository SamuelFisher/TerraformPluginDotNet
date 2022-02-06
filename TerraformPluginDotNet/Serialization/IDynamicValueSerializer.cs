using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TerraformPluginDotNet.Serialization;

public interface IDynamicValueSerializer
{
    T DeserializeJson<T>(ReadOnlyMemory<byte> value);

    T DeserializeMsgPack<T>(ReadOnlyMemory<byte> value);

    byte[] SerializeMsgPack<T>(T value);
}
