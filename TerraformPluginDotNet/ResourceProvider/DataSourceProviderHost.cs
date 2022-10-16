using TerraformPluginDotNet.Serialization;
using Tfplugin5;

namespace TerraformPluginDotNet.ResourceProvider;

class DataSourceProviderHost<T>
{
    private readonly IDataSourceProvider<T> _dataSourceProvider;
    private readonly IDynamicValueSerializer _serializer;

    public DataSourceProviderHost(
        IDataSourceProvider<T> dataSourceProvider,
        IDynamicValueSerializer serializer)
    {
        _dataSourceProvider = dataSourceProvider;
        _serializer = serializer;
    }

    public async Task<ReadDataSource.Types.Response> ReadDataSource(ReadDataSource.Types.Request request)
    {
        var current = DeserializeDynamicValue(request.Config);

        var read = await _dataSourceProvider.ReadAsync(current);
        var readSerialized = SerializeDynamicValue(read);

        return new ReadDataSource.Types.Response
        {
            State = readSerialized,
        };
    }

    private T DeserializeDynamicValue(DynamicValue value)
    {
        if (!value.Msgpack.IsEmpty)
        {
            return _serializer.DeserializeMsgPack<T>(value.Msgpack.Memory);
        }

        if (!value.Json.IsEmpty)
        {
            return _serializer.DeserializeJson<T>(value.Json.Memory);
        }

        throw new ArgumentException("Either MessagePack or Json must be non-empty.", nameof(value));
    }

    private DynamicValue SerializeDynamicValue(T value)
    {
        return new DynamicValue { Msgpack = Google.Protobuf.ByteString.CopyFrom(_serializer.SerializeMsgPack(value)) };
    }
}
