using System.Text;
using MessagePack;
using MessagePack.Formatters;

namespace TerraformPluginDotNet.Serialization;

public class ComputedValueFormatter : IMessagePackFormatter<string>
{
    public string Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
    {
        if (reader.TryReadNil())
        {
            return null;
        }
        else if (reader.NextMessagePackType == MessagePackType.Extension && reader.TryReadExtensionFormatHeader(out var extHeader) && extHeader.TypeCode == 0)
        {
            reader.Skip();
            return null;
        }

        return reader.ReadString();
    }

    public void Serialize(ref MessagePackWriter writer, string value, MessagePackSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteExtensionFormat(new ExtensionResult(0, new byte[1]));
            return;
        }

        writer.WriteString(Encoding.UTF8.GetBytes(value));
    }
}
