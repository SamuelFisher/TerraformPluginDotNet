using System.ComponentModel;
using MessagePack;

namespace DataSourceProvider;

[MessagePackObject]
public class Configuration
{
    [Key("dummy_data")]
    [Description("Dummy data returned by the data source provider.")]
    public string? Data { get; set; }
}
