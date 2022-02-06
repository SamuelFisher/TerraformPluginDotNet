using System.Threading.Tasks;
using TerraformPluginDotNet.ProviderConfig;

namespace SampleProvider;

public class SampleConfigurator : IProviderConfigurator<Configuration>
{
    public Configuration Config { get; private set; }

    public Task ConfigureAsync(Configuration config)
    {
        Config = config;
        return Task.CompletedTask;
    }
}
